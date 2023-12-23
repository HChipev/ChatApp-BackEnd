namespace Data.ViewModels.Admin.Models
{
    public class PermissionsViewModel
    {
        public IEnumerable<PermissionSimpleViewModel> Permissions { get; set; } = new List<PermissionSimpleViewModel>();
    }
}