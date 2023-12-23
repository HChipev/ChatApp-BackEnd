using AutoMapper;
using Common.Classes;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.Admin.Models;
using Data.ViewModels.BasicResponseModels;
using Microsoft.AspNetCore.SignalR;
using Service.Hubs;
using Service.Interfaces;

namespace Service.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IHubContext<RefetchAdminDataHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<RolePermission> _rolePermissionsRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;

        public AdminService(IRepository<User> userRepository, IRepository<Role> roleRepository, IMapper mapper,
            IRepository<UserRole> userRoleRepository, IHubContext<RefetchAdminDataHub> hubContext,
            IRepository<Permission> permissionRepository, IRepository<RolePermission> rolePermissionsRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _userRoleRepository = userRoleRepository;
            _hubContext = hubContext;
            _permissionRepository = permissionRepository;
            _rolePermissionsRepository = rolePermissionsRepository;
        }

        public ServiceResult<UsersViewModel> GetUsers()
        {
            var users = _userRepository.GetAll().ToList();
            var usersViewModel = _mapper.Map<UsersViewModel>(users);

            foreach (var user in usersViewModel.Users)
            {
                user.UserRoles =
                    _mapper.Map<List<UserRoleViewModel>>(
                        _userRoleRepository.FindAllByCondition(y => y.UserId == user.Id));
            }

            return new ServiceResult<UsersViewModel>
            {
                IsSuccess = true,
                Data = usersViewModel,
                Message = ""
            };
        }

        public ServiceResult<RolesViewModel> GetRoles()
        {
            var roles = _roleRepository.GetAll().ToList();
            var rolesViewModel = _mapper.Map<RolesViewModel>(roles);

            foreach (var role in rolesViewModel.Roles)
            {
                role.RolePermissions =
                    _mapper.Map<List<RolePermissionViewModel>>(
                        _rolePermissionsRepository.FindAllByCondition(y => y.RoleId == role.Id));
            }

            return new ServiceResult<RolesViewModel>
            {
                IsSuccess = true,
                Data = rolesViewModel,
                Message = ""
            };
        }

        public ServiceResult<PermissionsViewModel> GetPermissions()
        {
            var permissions = _permissionRepository.GetAll().ToList();

            return new ServiceResult<PermissionsViewModel>
            {
                IsSuccess = true,
                Data = _mapper.Map<PermissionsViewModel>(permissions),
                Message = ""
            };
        }

        public async Task<ServiceResult<BasicResponseViewModel>> UpdateRolePermissionsAsync(
            RolePermissionsViewModel model,
            int loggedInUserId)
        {
            var roleId = model.RoleId;

            var existingRolePermissions =
                _rolePermissionsRepository.FindAllByCondition(x => x.RoleId == roleId).ToList();

            foreach (var entry in model.RolePermissions)
            {
                var existingRolePermission = existingRolePermissions
                    .FirstOrDefault(x => x.RoleId == entry.RoleId && x.PermissionId == entry.PermissionId);

                if (existingRolePermission is null)
                {
                    _rolePermissionsRepository.Add(_mapper.Map<RolePermission>(entry));
                }
            }

            foreach (var existingRolePermission in existingRolePermissions)
            {
                var viewModelPermissionExists = model.RolePermissions
                    .Any(x => x.RoleId == existingRolePermission.RoleId &&
                              x.PermissionId == existingRolePermission.PermissionId);

                if (!viewModelPermissionExists)
                {
                    _rolePermissionsRepository.DeleteByCondition(x =>
                        x.RoleId == existingRolePermission.RoleId &&
                        x.PermissionId == existingRolePermission.PermissionId);
                }
            }

            _rolePermissionsRepository.SaveChanges();

            await _hubContext.Clients.Group(loggedInUserId.ToString()).SendAsync("RefetchRoles");

            return new ServiceResult<BasicResponseViewModel>
            {
                IsSuccess = true,
                Data = new BasicResponseViewModel { Message = "Successfully update role permissions." },
                Message = ""
            };
        }

        public async Task<ServiceResult<BasicResponseViewModel>> DeleteRoleAsync(int roleId, int loggedInUserId)
        {
            var role = _roleRepository.FindByCondition(x => x.Id == roleId);

            if (role is null)
            {
                return new ServiceResult<BasicResponseViewModel>
                    { IsSuccess = false, Data = null, Message = "Role doesn't exists!" };
            }

            _roleRepository.Delete(role.Id);

            _roleRepository.SaveChanges();

            await _hubContext.Clients.Group(loggedInUserId.ToString()).SendAsync("RefetchRoles");

            return new ServiceResult<BasicResponseViewModel>
            {
                IsSuccess = true, Data = new BasicResponseViewModel { Message = "Successfully deleted role." },
                Message = ""
            };
        }

        public async Task<ServiceResult<BasicResponseViewModel>> DeleteUserAsync(int userId, int loggedInUserId)
        {
            var deletedUser = _userRepository.Delete(userId);

            if (deletedUser is null)
            {
                return new ServiceResult<BasicResponseViewModel>
                    { IsSuccess = false, Data = null, Message = "User doesn't exist!" };
            }

            _userRepository.SaveChanges();

            await _hubContext.Clients.Group(loggedInUserId.ToString()).SendAsync("RefetchUsers");

            return new ServiceResult<BasicResponseViewModel>
            {
                IsSuccess = true, Data = new BasicResponseViewModel { Message = "Successfully deleted user." },
                Message = ""
            };
        }

        public async Task<ServiceResult<BasicResponseViewModel>> AddRoleAsync(RoleSimpleViewModel model,
            int loggedInUserId)
        {
            var existingRole = _roleRepository.FindByCondition(x => x.Name == model.Name);

            if (existingRole is not null)
            {
                return new ServiceResult<BasicResponseViewModel>
                    { IsSuccess = false, Data = null, Message = "Role already exists!" };
            }

            var newRole = _mapper.Map<Role>(model);
            newRole.Permissions = new List<Permission>();

            foreach (var permissionId in model.RolePermissions.Select(x => x.PermissionId))
            {
                var permission = _permissionRepository.Find(permissionId);

                if (permission != null)
                {
                    newRole.Permissions.Add(permission);
                }
            }

            _roleRepository.Add(newRole);
            _roleRepository.SaveChanges();

            await _hubContext.Clients.Group(loggedInUserId.ToString()).SendAsync("RefetchRoles");

            return new ServiceResult<BasicResponseViewModel>
            {
                IsSuccess = true, Data = new BasicResponseViewModel { Message = "Successfully added role." },
                Message = ""
            };
        }

        public async Task<ServiceResult<BasicResponseViewModel>> UpdateUserRolesAsync(UserRolesViewModel model,
            int loggedInUserId)
        {
            var userId = model.UserId;

            var existingUserRoles = _userRoleRepository.FindAllByCondition(x => x.UserId == userId).ToList();

            foreach (var entry in model.UserRoles)
            {
                var existingUserRole = existingUserRoles
                    .FirstOrDefault(x => x.UserId == entry.UserId && x.RoleId == entry.RoleId);

                if (existingUserRole is null)
                {
                    _userRoleRepository.Add(_mapper.Map<UserRole>(entry));
                }
            }

            foreach (var existingUserRole in existingUserRoles)
            {
                var viewModelRoleExists = model.UserRoles
                    .Any(x => x.UserId == existingUserRole.UserId && x.RoleId == existingUserRole.RoleId);

                if (!viewModelRoleExists)
                {
                    _userRoleRepository.DeleteByCondition(x =>
                        x.UserId == existingUserRole.UserId && x.RoleId == existingUserRole.RoleId);
                }
            }

            _userRoleRepository.SaveChanges();

            await _hubContext.Clients.Group(loggedInUserId.ToString()).SendAsync("RefetchUsers");

            return new ServiceResult<BasicResponseViewModel>
            {
                IsSuccess = true, Data = new BasicResponseViewModel { Message = "Successfully update user roles." },
                Message = ""
            };
        }
    }
}