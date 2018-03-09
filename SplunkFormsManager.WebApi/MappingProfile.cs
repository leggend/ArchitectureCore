using AutoMapper;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplunkFormsManager.WebApi
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            //Dummy
            CreateMap<DummyDTO, DummyEntity>();
            CreateMap<DummyEntity, DummyDTO>();
        }
    }
}
