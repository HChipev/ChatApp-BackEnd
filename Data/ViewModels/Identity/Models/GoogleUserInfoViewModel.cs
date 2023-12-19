using Newtonsoft.Json;

namespace Data.ViewModels.Identity.Models
{
    public class GoogleUserInfoViewModel
    {
        public string Sub { get; set; }
        public string Name { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        public string Picture { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Locale { get; set; }
    }
}