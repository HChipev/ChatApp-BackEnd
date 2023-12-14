using AutoMapper;
using Data.Entities;
using Data.ViewModels.Admin.Models;

namespace Data.ViewModels.Admin.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleSimpleViewModel, Role>()
                .ForMember(dest => dest.Id
                    , opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedName
                    , opt => opt.MapFrom(src => src.Name.ToUpper()));
            CreateMap<Role, RoleSimpleViewModel>();
            CreateMap<List<Role>, RolesViewModel>().ForMember(dest => dest.Roles
                , opt => opt.MapFrom(src => src));
        }
    }
}