namespace Data.ViewModels.Admin.Models
{
    public class RolePermissionsViewModel
    {
        public IEnumerable<RolePermissionViewModel> RolePermissions { get; set; } = new List<RolePermissionViewModel>();
        public int RoleId { get; set; }
    }
}