using AutoMapper;
using Data.Entities;
using Data.ViewModels.Identity.Models;

namespace Data.ViewModels.Identity.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserLoginViewModel>().ReverseMap();
            CreateMap<User, UserRegistrationViewModel>().ReverseMap().ForMember(dest => dest.Name
                , opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<GoogleUserInfoViewModel, UserRegistrationViewModel>()
                .ForMember(dest => dest.FirstName
                    , opt => opt.MapFrom(src => src.GivenName))
                .ForMember(dest => dest.LastName
                    , opt => opt.MapFrom(src => src.FamilyName))
                .ForMember(dest => dest.Username
                    , opt => opt.MapFrom(src => src.Email));
        }
    }
}