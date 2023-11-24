using Data.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class UserRole : IdentityUserRole<int>, IBaseEntity
    {
    }
}