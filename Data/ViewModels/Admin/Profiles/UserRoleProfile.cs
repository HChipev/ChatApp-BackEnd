using AutoMapper;
using Data.Entities;
using Data.ViewModels.Admin.Models;

namespace Data.ViewModels.Admin.Profiles
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {
            CreateMap<UserRoleViewModel, UserRole>().ReverseMap();
        }
    }
}