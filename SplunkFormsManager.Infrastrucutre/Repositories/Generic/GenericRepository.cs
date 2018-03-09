using Microsoft.EntityFrameworkCore;
using SplunkFormsManager.CrossCutting.Utilities;
using SplunkFormsManager.Domain.Entities;
using SplunkFormsManager.Domain.RepositoryContracts.Generic;
using SplunkFormsManager.Infrastrucutre.Context;
using SplunkFormsManager.Infrastrucutre.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplunkFormsManager.Infrastrucutre.Repositories.Generic
{
    public class GenericRepository<E> : IGenericRepository<E> where E : BaseEntity
    {
        private readonly DataContext _dbCtx = null;

        public GenericRepository(DataContext dbCtx)
        {
            _dbCtx = dbCtx;
        }
        public async Task<bool> ExistsAsync(long id)
        {
            return await _dbCtx.Set<E>().AnyAsync<E>(c => c.Id == id);
        }

        public async Task<IEnumerable<E>> FindAsync(List<LinqOrderRule> orders, List<LinqFilterRule> rules, string[] includes)
        {
            var result = _dbCtx.Set<E>().AsQueryable();
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    result = result.Include(include);
                }
            }

            if (orders != null && orders.Count > 0)
            {
                result = LinqExpresions.OrderByRules(result, orders);
            }

            if (rules != null && rules.Count > 0)
            {
                return LinqExpresions.FilterByRulesRecursivly<E>(result, rules);
            }
            else
            {
                return result.AsEnumerable();
            }
        }

        public async Task<IEnumerable<E>> FindAsync(string[] includes)
        {
            var result = _dbCtx.Set<E>().AsQueryable();
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    result = result.Include(include);
                }
            }

            return result.AsEnumerable();
        }

        public async Task<E> FindByIdAsync(long id)
        {
            return await _dbCtx.Set<E>().FindAsync(id);
        }

        public async Task<E> FindByIdAsync(long id, List<string> includes)
        {
            var dbset = _dbCtx.Set<E>().AsQueryable();
            foreach (string include in includes)
            {
                dbset = dbset.Include(include);
            }

            return await dbset.SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<long> AddAsync(E entity)
        {
            try
            {
                _dbCtx.Entry(entity);
                _dbCtx.Entry<E>(entity).State = EntityState.Added;
                await _dbCtx.SaveChangesAsync();
                _dbCtx.Entry<E>(entity).State = EntityState.Detached;
                return entity.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateAsync(E entity)
        {
            _dbCtx.Entry<E>(entity).State = EntityState.Modified;
            await _dbCtx.SaveChangesAsync();
            _dbCtx.Entry<E>(entity).State = EntityState.Detached;
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _dbCtx.Set<E>().FindAsync(id);
            if (entity != null)
            {
                _dbCtx.Entry<E>(entity).State = EntityState.Deleted;
                await _dbCtx.SaveChangesAsync();
                _dbCtx.Entry<E>(entity).State = EntityState.Detached;
            }

        }
    }
}
