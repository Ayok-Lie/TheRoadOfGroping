using RoadOfGroping.Common.Consts;
using RoadOfGroping.Core.Permissions.DomainService;
using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.ToolService
{
    public class ToolAppService : ApplicationService
    {
        private readonly IPermissionRoleRelationManager permissionRoleRelationManager;
        private readonly IPermissionsManager permissionManager;

        private readonly List<string> list = new List<string>()
            {
                "App.User.Query",
                "App.User.Create",
                "App.User.Update",
                "App.User.Delete"
            };

        public ToolAppService(IServiceProvider serviceProvider, IPermissionRoleRelationManager permissionRoleRelationManager, IPermissionsManager permissionManager) : base(serviceProvider)
        {
            this.permissionRoleRelationManager = permissionRoleRelationManager;
            this.permissionManager = permissionManager;
        }

        public async Task CreatePermissionRoleRelation()
        {
            foreach (var item in list)
            {
                var relation = new PermissionRoleRelation();
                relation.PermissionCode = item;
                relation.IsGranted = true;
                relation.RoleId = RoadOfGropingConst.DefaultRoleId;
                await permissionRoleRelationManager.Create(relation);
            }
        }
    }
}