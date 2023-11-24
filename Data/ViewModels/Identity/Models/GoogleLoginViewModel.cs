using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.Identity.Models
{
    public class GoogleLoginViewModel
    {
        [Required(ErrorMessage = "GoogleId is required!")]
        public string GoogleId { get; set; }
    }
}