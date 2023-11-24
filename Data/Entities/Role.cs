using Data.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class Role : IdentityRole<int>, IBaseEntity
    {
    }
}