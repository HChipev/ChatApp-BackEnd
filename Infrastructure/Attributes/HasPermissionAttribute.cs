using Common.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Attributes
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission) : base(permission.ToString())
        {
        }
    }
}