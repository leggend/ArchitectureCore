using SplunkFormsManager.CrossCutting.Utilities;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplunkFormsManager.Domain.Contracts.Generic
{
    public interface IGenericLibrary<D,E>
        where D: BaseDTO
        where E: BaseEntity
    {
        Task<bool> ExistAsync(long id);
        Task<IList<D>> GetItemsAsync(CommonFilter filter);
        Task<D> GetItemAsync(long id);
        Task<D> GetItemAsync(long id, List<string> includes);
        Task<PageListDTO<D>> GetPageList(CommonFilter filter);
        Task<long> SaveAsync(D item);
        Task RemoveAsync(long id);
    }
}
