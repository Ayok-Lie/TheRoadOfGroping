using System.Globalization;
using RoadOfGroping.Application.FeatureService.Dtos;
using RoadOfGroping.Core.Permissions.DomainService;
using RoadOfGroping.Core.Users.DomainService;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.FeatureService
{
    public class FeatureAppService : ApplicationService, IFeatureAppService
    {
        private readonly IPermissionRoleRelationManager _permissionRoleRelationManager;
        private readonly IUserRolesManager _roleUsersManager;

        public FeatureAppService(IServiceProvider serviceProvider,
            IPermissionRoleRelationManager permissionRoleRelationManager,
            IUserRolesManager roleUsersManager)
            : base(serviceProvider)
        {
            _permissionRoleRelationManager = permissionRoleRelationManager;
            _roleUsersManager = roleUsersManager;
        }

        public async Task<FeatureListDto> GetUserConfigurations()
        {
            var userId = UserSession.UserId;
            var roleIds = await _roleUsersManager.GetUserRoleIdsAsync(userId);
            var userPermissions = await _permissionRoleRelationManager.GetUserAllPermissionsAsync(roleIds);
            var currentCulture = CultureInfo.CurrentCulture;
            return new FeatureListDto
            {
                Roles = roleIds,
                Permissions = userPermissions
            };
        }
    }
}