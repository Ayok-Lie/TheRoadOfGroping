using Microsoft.AspNetCore.Authorization;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequirePermissions(this AuthorizationPolicyBuilder builder, params string[] permissions)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            return builder.AddRequirements(new PermissionRequirement(permissions));
        }
    }
}