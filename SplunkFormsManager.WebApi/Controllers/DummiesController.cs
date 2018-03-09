using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using SplunkFormsManager.WebApi.Controllers.Generic;
using SplunkFormsManager.Application.Contracts.Generic;

namespace SplunkFormsManager.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Dummies")]
    public class DummiesController : GenericController<DummyDTO, DummyEntity>
    {
        public DummiesController(IGenericService<DummyDTO, DummyEntity> service) : base(service)
        {
        }

    }
}