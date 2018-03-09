using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using SplunkFormsManager.Application.Contracts.Generic;
using SplunkFormsManager.CrossCutting.Utilities;

namespace SplunkFormsManager.WebApi.Controllers.Generic
{
    [Produces("application/json")]
    [Route("api/Generic")]
    public class GenericController<D,E> : Controller
        where D : BaseDTO
        where E : BaseEntity
    {
        private readonly IGenericService<D, E> _service = null;

        public GenericController(IGenericService<D, E> service)
        {
            _service = service;
        }
        // GET: api/GenericApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var item = await _service.GetItemAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/GenericApi/5
        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery] CommonFilter filter)
        {
            if (filter == null) filter = new CommonFilter();
            if (!string.IsNullOrEmpty(filter.JSonRules))
            {
                var rules = (LinqFilterRule[])Newtonsoft.Json.JsonConvert.DeserializeObject(filter.JSonRules, typeof(LinqFilterRule[]));
                filter.FilterRules = rules;
            }

            if (!string.IsNullOrEmpty(filter.JSonOrders))
            {
                filter.OrderRules = (LinqOrderRule[])Newtonsoft.Json.JsonConvert.DeserializeObject(filter.JSonOrders, typeof(LinqOrderRule[]));
            }

            if (filter.ResultList)
            {
                var items = await _service.GetItemsAsync(filter);
                return Ok(items);
            }
            else
            {
                var pageList = await _service.GetPageList(filter);
                return Ok(pageList);
            }
        }

        // POST: api/GenericApi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]D value)
        {
            if (value == null) return BadRequest("value can't be null.");
            value.Id = await _service.SaveAsync(value);
            return Ok(value);
        }

        // PUT: api/GenericApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody]D value)
        {
            if (id <= 0) return BadRequest("id can't be 0.");
            if (value == null) return BadRequest("value can't be null.");
            if (id != value.Id) return BadRequest("param id and value.id are diferent.");
            if (!await _service.ExistAsync(id)) return NotFound();

            await _service.SaveAsync(value);

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0) return BadRequest();
            if (!await _service.ExistAsync(id)) return NotFound();

            await _service.RemoveAsync(id);
            return Ok();
        }
    }
}
