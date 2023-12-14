using AutoMapper;
using Data.Entities;
using Data.ViewModels.Admin.Models;

namespace Data.ViewModels.Admin.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserSimpleViewModel>();
            CreateMap<List<User>, UsersViewModel>().ForMember(dest => dest.Users
                , opt => opt.MapFrom(src => src));
        }
    }
}