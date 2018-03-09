using SplunkFormsManager.Application.Contracts.Generic;
using SplunkFormsManager.CrossCutting.Utilities;
using SplunkFormsManager.Domain.Contracts.Generic;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplunkFormsManager.Application.Services.Generic
{
    public class GenericService<D, E> : IGenericService<D, E>
        where D : BaseDTO
        where E : BaseEntity
    {
        private readonly IGenericLibrary<D, E> _library = null;

        public GenericService(IGenericLibrary<D, E> library)
        {
            _library = library;
        }
        public async Task<bool> ExistAsync(long id)
        {
            return await _library.ExistAsync(id);
        }

        public async Task<D> GetItemAsync(long id)
        {
            return await _library.GetItemAsync(id);
        }

        public async Task<IList<D>> GetItemsAsync(CommonFilter filter)
        {
            return await _library.GetItemsAsync(filter);
        }

        public async Task<D> GetItemAsync(long id, List<string> includes)
        {
            return await _library.GetItemAsync(id, includes);
        }


        public async Task<PageListDTO<D>> GetPageList(CommonFilter filter)
        {
            return await _library.GetPageList(filter);
        }

        public async Task RemoveAsync(long id)
        {
            await _library.RemoveAsync(id);
        }

        public async Task<long> SaveAsync(D item)
        {
            return await _library.SaveAsync(item);
        }
    }
}
