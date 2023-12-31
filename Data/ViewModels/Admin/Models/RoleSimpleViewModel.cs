namespace Data.ViewModels.Admin.Models
{
    public class RoleSimpleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public IEnumerable<RolePermissionViewModel> RolePermissions { get; set; } = new List<RolePermissionViewModel>();
    }
}