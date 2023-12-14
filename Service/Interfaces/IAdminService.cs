using Common.Classes;
using Data.ViewModels.Admin.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IAdminService : IService
    {
        public Task<ServiceResult<bool>> DeleteUserAsync(int userId, int loggedInUserId);
        public Task<ServiceResult<bool>> AddRoleAsync(RoleSimpleViewModel model, int loggedInUserId);
        public Task<ServiceResult<bool>> DeleteRoleAsync(int roleId, int loggedInUserId);

        public Task<ServiceResult<bool>>
            UpdateUserRolesAsync(UserRolesViewModel model, int loggedInUserId);

        public ServiceResult<UsersViewModel> GetUsers();
        public ServiceResult<RolesViewModel> GetRoles();
    }
}