using AutoMapper;
using BiteByByteAPI.Models;
using BiteByByteAPI.Entities;
using BiteByByteAPI.Models.Users;

namespace BiteByByteAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}