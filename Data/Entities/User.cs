using Data.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class User : IdentityUser<int>, IBaseEntity
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string Picture { get; set; } = "https://i.stack.imgur.com/l60Hf.png";

        public string Name { get; set; }

        public ICollection<Conversation> Conversations { get; set; }

        public ICollection<Role> Roles { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}