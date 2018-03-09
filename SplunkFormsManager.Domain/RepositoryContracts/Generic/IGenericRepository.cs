using SplunkFormsManager.CrossCutting.Utilities;
using SplunkFormsManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplunkFormsManager.Domain.RepositoryContracts.Generic
{
    public interface IGenericRepository<E> where E : BaseEntity
    {
        Task<bool> ExistsAsync(long id);
        Task<IEnumerable<E>> FindAsync(string[] includes);
        Task<IEnumerable<E>> FindAsync(List<LinqOrderRule> orders, List<LinqFilterRule> rules, string[] includes);
        Task<E> FindByIdAsync(long id);
        Task<E> FindByIdAsync(long id, List<string> includes);
        Task<long> AddAsync(E entity);
        Task UpdateAsync(E entity);
        Task DeleteAsync(long id);
    }
}
