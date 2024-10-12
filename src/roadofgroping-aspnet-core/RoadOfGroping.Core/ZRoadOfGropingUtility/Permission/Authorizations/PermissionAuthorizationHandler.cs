using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.Permissions.DomainService;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>, ISingletonDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionRoleRelationManager _permissionRoleRelationManager;

        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IPermissionRoleRelationManager permissionRoleRelationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionRoleRelationManager = permissionRoleRelationManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!requirement.Permissions.Any())
            {
                context.Succeed(requirement);
            }
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //AuthorizationFailureReason failureReason;
            //判断是否通过鉴权中间件--是否登录
            if (userId is null || !isAuthenticated)
            {
                //failureReason = new AuthorizationFailureReason(this, "请登录到系统");
                //context.Fail(failureReason);
                //return;
                throw new Exception("请登录到系统");
            }

            var str = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (str != null)
            {
                var roleId = str.Split(',').ToList();
                var userPermissions = await _permissionRoleRelationManager.QueryAsNoTracking
                        .Where(x => roleId.Contains(x.RoleId))
                        .Select(x => x.PermissionCode).ToListAsync();
                foreach (var permission in requirement.Permissions)
                {
                    if (userPermissions.Contains(permission))
                    {
                        context.Succeed(requirement);
                        break;
                    }
                    else
                    {
                        //failureReason = new AuthorizationFailureReason(this, $"权限不足，无法请求--请求接口{_httpContextAccessor.HttpContext?.Request?.Path ?? ""}");
                        //context.Fail(failureReason);
                        //return;
                        throw new Exception($"权限不足，无法请求--请求接口{_httpContextAccessor.HttpContext?.Request?.Path}");
                    }
                }
            }
        }
    }
}