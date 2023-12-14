namespace Data.ViewModels.Admin.Models
{
    public class UserSimpleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Picture { get; set; } = "";
        public IEnumerable<UserRoleViewModel> UserRoles { get; set; } = new List<UserRoleViewModel>();
    }
}