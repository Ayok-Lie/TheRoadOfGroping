using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly DataSeeds _dataSeeds;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(DataSeeds dataSeeds, IHttpContextAccessor httpContextAccessor)
        {
            _dataSeeds = dataSeeds;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!requirement.Permissions.Any())
            {
                context.Succeed(requirement);
            }
            // 获取用户名
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            var userPermissions = _dataSeeds.GetUserPermissions(context.User.FindFirst(ClaimTypes.Role)?.Value);
            foreach (var permission in requirement.Permissions)
            {
                if (userPermissions.Contains(permission))
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}