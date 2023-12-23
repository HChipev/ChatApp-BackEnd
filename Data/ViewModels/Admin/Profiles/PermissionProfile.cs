using AutoMapper;
using Data.Entities;
using Data.ViewModels.Admin.Models;

namespace Data.ViewModels.Admin.Profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<List<Permission>, PermissionsViewModel>().ForMember(dest => dest.Permissions
                , opt => opt.MapFrom(src => src));
            CreateMap<Permission, PermissionSimpleViewModel>();
            CreateMap<RolePermission, RolePermissionViewModel>().ReverseMap();
        }
    }
}