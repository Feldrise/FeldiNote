using AutoMapper;
using FeldiNote.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeldiNote.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegister, User>();
            CreateMap<UserAuthenticat, User>();
        }
    }
}
