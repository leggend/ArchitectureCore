using AutoMapper;
using SplunkFormsManager.CrossCutting.Utilities;
using SplunkFormsManager.Domain.Contracts.Generic;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using SplunkFormsManager.Domain.RepositoryContracts.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplunkFormsManager.Domain.Libraries.Generic
{
    public class GenericLibrary<D, E> : IGenericLibrary<D, E>
        where D : BaseDTO
        where E : BaseEntity
    {
        private readonly IGenericRepository<E> _repo = null;
        private readonly IMapper _mapper = null;
        public GenericLibrary(IMapper mapper, IGenericRepository<E> repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<bool> ExistAsync(long id)
        {
            return await _repo.ExistsAsync(id);
        }

        public async Task<D> GetItemAsync(long id)
        {
            return _mapper.Map<D>(await _repo.FindByIdAsync(id));
        }

        public async Task<D> GetItemAsync(long id, List<string> includes)
        {
            return _mapper.Map<D>(await _repo.FindByIdAsync(id, includes));
        }

        public async Task<IList<D>> GetItemsAsync(CommonFilter filter)
        {
            List<LinqOrderRule> orders = new List<LinqOrderRule>();
            if (filter != null && filter.OrderRules != null) orders = filter.OrderRules.ToList();

            List<LinqFilterRule> rules = new List<LinqFilterRule>();
            if (filter != null && filter.FilterRules != null) rules = filter.FilterRules.ToList();

            var items = await _repo.FindAsync(orders, rules, filter.Includes);
            return _mapper.Map<IList<D>>(items.ToList());
        }

        public async Task<PageListDTO<D>> GetPageList(CommonFilter filter)
        {
            PageListDTO<D> result = new PageListDTO<D>();
            result.CurrentPage = 0;
            result.TotalPages = 1;

            List<LinqOrderRule> orders = new List<LinqOrderRule>();
            if (filter != null && filter.OrderRules != null) orders = filter.OrderRules.ToList();

            List<LinqFilterRule> rules = new List<LinqFilterRule>();
            if (filter != null && filter.FilterRules != null) rules = filter.FilterRules.ToList();

            var items = await _repo.FindAsync(orders, rules, filter.Includes);

            int totalItems = items.Count();

            if (filter.ItemsPerPage != null && filter.IndexPage >= 0)
            {
                result.ItemsPerPages = filter.ItemsPerPage.Value;
                result.CurrentPage = filter.IndexPage.Value;
                if (totalItems > filter.ItemsPerPage)
                {
                    decimal pages = totalItems / filter.ItemsPerPage.Value;
                    decimal numpages = Math.Truncate(pages);
                    if ((numpages * filter.ItemsPerPage.Value) < totalItems) result.TotalPages = Convert.ToInt32(numpages) + 1;
                    else result.TotalPages = Convert.ToInt32(numpages);

                    if (result.TotalPages < filter.IndexPage) filter.IndexPage = result.TotalPages - 1;

                    items = items.Skip(filter.IndexPage.Value * filter.ItemsPerPage.Value).Take(filter.ItemsPerPage.Value);
                }
            }
            else
            {
                result.ItemsPerPages = totalItems;
            }

            result.Items = _mapper.Map<IList<D>>(items.ToList());

            return result;
        }


        public async Task RemoveAsync(long id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<long> SaveAsync(D item)
        {
            if (item.Id == 0)
            {
                await _repo.AddAsync(_mapper.Map<E>(item));
            }
            else
            {
                await _repo.UpdateAsync(_mapper.Map<E>(item));
            }

            return item.Id;
        }


    }
}
