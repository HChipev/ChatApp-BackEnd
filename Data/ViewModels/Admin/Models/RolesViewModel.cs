namespace Data.ViewModels.Admin.Models
{
    public class RolesViewModel
    {
        public IEnumerable<RoleSimpleViewModel> Roles { get; set; } = new List<RoleSimpleViewModel>();
    }
}