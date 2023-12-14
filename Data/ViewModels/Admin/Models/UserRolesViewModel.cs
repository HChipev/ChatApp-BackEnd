namespace Data.ViewModels.Admin.Models
{
    public class UserRolesViewModel
    {
        public IEnumerable<UserRoleViewModel> UserRoles { get; set; } = new List<UserRoleViewModel>();
        public int UserId { get; set; }
    }
}