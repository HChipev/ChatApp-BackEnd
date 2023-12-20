using Common.Classes;
using Data.ViewModels.Admin.Models;
using Data.ViewModels.BasicResponseModels;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IAdminService : IService
    {
        public Task<ServiceResult<BasicResponseViewModel>> DeleteUserAsync(int userId, int loggedInUserId);
        public Task<ServiceResult<BasicResponseViewModel>> AddRoleAsync(RoleSimpleViewModel model, int loggedInUserId);
        public Task<ServiceResult<BasicResponseViewModel>> DeleteRoleAsync(int roleId, int loggedInUserId);

        public Task<ServiceResult<BasicResponseViewModel>>
            UpdateUserRolesAsync(UserRolesViewModel model, int loggedInUserId);

        public ServiceResult<UsersViewModel> GetUsers();
        public ServiceResult<RolesViewModel> GetRoles();
    }
}