using Microsoft.AspNetCore.Authorization;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(params string[] permissions) =>
            Permissions = permissions ?? Array.Empty<string>();

        public string[] Permissions { get; set; }
    }
}