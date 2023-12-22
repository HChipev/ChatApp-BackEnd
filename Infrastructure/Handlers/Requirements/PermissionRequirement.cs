using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Handlers.Requirements
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; }
    }
}