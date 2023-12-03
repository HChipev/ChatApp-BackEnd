using Data.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class Role : IdentityRole<int>, IBaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}