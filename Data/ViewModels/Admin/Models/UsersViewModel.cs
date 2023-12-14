namespace Data.ViewModels.Admin.Models
{
    public class UsersViewModel
    {
        public IEnumerable<UserSimpleViewModel> Users { get; set; } = new List<UserSimpleViewModel>();
    }
}