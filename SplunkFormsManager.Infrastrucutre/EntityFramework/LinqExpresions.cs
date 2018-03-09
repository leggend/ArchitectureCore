using SplunkFormsManager.CrossCutting;
using SplunkFormsManager.CrossCutting.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SplunkFormsManager.Infrastrucutre.EntityFramework
{
    static public class LinqExpresions
    {

        static MethodInfo OrderByMethodInfo;
        static MethodInfo OrderByDescendingMethodInfo;
        static MethodInfo ThenByMethodInfo;
        static MethodInfo ThenByDescendingMethodInfo;
        static MethodInfo ContainsMethodInfo;

        /// <summary>
        /// Static constructor. Initialize Reflection informations about Order operators
        /// </summary>
        static LinqExpresions()
        {
            OrderByMethodInfo = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "OrderBy" && m.GetParameters().Count() == 2);
            OrderByDescendingMethodInfo = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Count() == 2);
            ThenByMethodInfo = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "ThenBy" && m.GetParameters().Count() == 2);
            ThenByDescendingMethodInfo = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "ThenByDescending" && m.GetParameters().Count() == 2);

            ContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        }

        /// <summary>
        /// Order <paramref name="dataCollection"/> according to <paramref name="rules"/> sequence
        /// </summary>
        /// <typeparam name="T">Collection item type</typeparam>
        /// <param name="dataCollection">Queryable collection</param>
        /// <param name="rules">Order rules to apply</param>
        /// <returns>Ordered queryable collection</returns>
        public static IOrderedQueryable<T> OrderByRules<T>(this IQueryable<T> dataCollection, IEnumerable<LinqOrderRule> rules)
        {
            if (!rules.Any())
                throw new ArgumentException("Rules list is empty", "rules");
            // apply first rule (special case: use OrderBy operator and not ThenBy)
            LinqOrderRule rule = rules.First();
            MethodInfo orderOperator = rule.SortDirection == Const.ORDER_ASC ? OrderByDescendingMethodInfo : OrderByMethodInfo;
            IOrderedQueryable<T> orderedDataCollection = OrderByFieldOrPropertyName(dataCollection, orderOperator, rule.SortBy);
            // apply next rules recursivly
            return OrderByRulesRecursivly(orderedDataCollection, rules.Skip(1).ToList());
        }

        static IOrderedQueryable<T> OrderByFieldOrPropertyName<T>(IEnumerable<T> dataCollection, MethodInfo orderOperator, string fieldOrPropertyName)
        {
            // member corresponding to fieldOrPropertyName
            MemberInfo memberInfo = typeof(T).GetField(fieldOrPropertyName);
            Type memberType = null;
            if (memberInfo == null)
            {
                fieldOrPropertyName = FirstCharToUpper(fieldOrPropertyName);
                memberInfo = typeof(T).GetProperty(fieldOrPropertyName);
            }
            else
                memberType = (memberInfo as FieldInfo).FieldType;
            if (memberInfo == null)
                throw new ArgumentException(String.Format("Field or property '{0}' doesn't exist on type '{1}'", fieldOrPropertyName, typeof(T)));
            else
                memberType = (memberInfo as PropertyInfo).PropertyType;
            // build lambda expression: item => item.fieldName
            ParameterExpression paramExp = Expression.Parameter(typeof(T));
            LambdaExpression keySelectorExp = Expression.Lambda(Expression.MakeMemberAccess(paramExp, memberInfo), paramExp);
            // build concrete MethodInfo from the generic one
            orderOperator = orderOperator.MakeGenericMethod(typeof(T), memberType);
            // invoke method on dataCollection
            return orderOperator.Invoke(null, new object[] {
            dataCollection,
            keySelectorExp
        }) as IOrderedQueryable<T>;
        }

        static IOrderedQueryable<T> OrderByRulesRecursivly<T>(IOrderedQueryable<T> dataCollection, List<LinqOrderRule> rules)
        {
            if (!rules.Any())
                return dataCollection;
            // apply first rule
            LinqOrderRule rule = rules.First();
            MethodInfo orderOperator = rule.SortDirection == Const.ORDER_ASC ? ThenByDescendingMethodInfo : ThenByMethodInfo;
            IOrderedQueryable<T> orderedDataCollection = OrderByFieldOrPropertyName(dataCollection, orderOperator, rule.SortBy);
            // apply next rules recursivly
            return OrderByRulesRecursivly(orderedDataCollection, rules.Skip(1).ToList());
        }

        public static IEnumerable<T> FilterByRulesRecursivly<T>(IEnumerable<T> dataCollection, List<LinqFilterRule> rules)
        {
            if (!rules.Any())
                return dataCollection;
            // apply first rule
            LinqFilterRule rule = rules.First();
            IEnumerable<T> filteredDataCollection = FilterByRule<T>(dataCollection, rule);
            return FilterByRulesRecursivly(filteredDataCollection, rules.Skip(1).ToList());
        }

        static IEnumerable<T> FilterByRule<T>(IEnumerable<T> dataCollection, LinqFilterRule rule)
        {
            var lambdaWhere = LambdaWhere<T>(rule);
            return dataCollection.Where(lambdaWhere.Compile());
        }

        private static Expression<Func<T, bool>> LambdaWhere<T>(LinqFilterRule filterRule)
        {
            string fieldName = filterRule.Property;
            object fieldValue = filterRule.Value;
            string comparer = filterRule.Comparison;

            var parameter = Expression.Parameter(typeof(T), "t");
            var property = Expression.Property(parameter, fieldName);
            var value = Expression.Constant(fieldValue);
            var converted = Expression.Convert(value, property.Type);
            Expression<Func<T, bool>> lambda = null;

            switch (comparer)
            {
                case Const.COMPARISON_NOT_EQUAL:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(property, converted), parameter);
                    break;
                case Const.COMPARISON_GREATER:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(property, converted), parameter);
                    break;
                case Const.COMPARISON_GREATER_OR_EQUAL:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(property, converted), parameter);
                    break;
                case Const.COMPARISON_LESS:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.LessThan(property, converted), parameter);
                    break;
                case Const.COMPARISON_LESS_OR_EQUAL:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(property, converted), parameter);
                    break;
                case Const.COMPARISON_IS_NULL:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(property, null), parameter);
                    break;
                case Const.COMPARISON_IS_NOT_NULL:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(property, null), parameter);
                    break;
                case Const.COMPARISON_IN:
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.Call(property, ContainsMethodInfo, converted), parameter);
                    break;
                case Const.COMPARISON_NOT_IN:
                    MethodInfo methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var contains = Expression.Call(property, ContainsMethodInfo, converted);
                    var notContains = Expression.Not(contains);
                    lambda = Expression.Lambda<Func<T, bool>>(notContains, parameter);
                    break;
                default:
                    try
                    {
                        lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(property, converted), parameter);
                    }
                    catch (Exception ex)
                    {
                        string mess = ex.Message;
                        throw ex;
                    }
                    break;
            }

            return lambda;
        }


        static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
