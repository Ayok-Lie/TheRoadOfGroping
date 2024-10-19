using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.Permissions.DomainService;
using RoadOfGroping.Core.Roles.Entity.DomainService;
using RoadOfGroping.Core.Users.DomainService;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>, ISingletonDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionRoleRelationManager _permissionRoleRelationManager;
        private readonly IUserRolesManager _userRolesManager;

        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor,
            IPermissionRoleRelationManager permissionRoleRelationManager, 
            IUserRolesManager userRolesManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionRoleRelationManager = permissionRoleRelationManager;
            _userRolesManager = userRolesManager;
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
            var roleIds = await _userRolesManager.GetUserRoleIdsAsync(userId);
            var userPermissions = await _permissionRoleRelationManager.GetUserAllPermissionsAsync(roleIds);
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