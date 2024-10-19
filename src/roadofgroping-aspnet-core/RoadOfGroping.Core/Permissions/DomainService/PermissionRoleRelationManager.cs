using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Permissions.DomainService
{
    public class PermissionRoleRelationManager : AnotherDomainService<PermissionRoleRelation, Guid>, IPermissionRoleRelationManager
    {
        private readonly IPermissionsManager _permissionsManager;

        public PermissionRoleRelationManager(IServiceProvider serviceProvider, IPermissionsManager permissionsManager) : base(serviceProvider)
        {
            _permissionsManager = permissionsManager;
        }

        public async Task<PermissionRoleRelation> CreateAsync(PermissionRoleRelation permissionRole)
        {
            return await Create(permissionRole);
        }

        public async Task<PermissionRoleRelation> UpdateAsync(PermissionRoleRelation permissionRole)
        {
            return await Update(permissionRole);
        }

        public async Task<List<string>> GetUserAllPermissionsAsync(List<string> roleIds)
        {
            var permissionIds = await QueryAsNoTracking.Where(x => roleIds.Contains(x.RoleId)).Select(x => x.PermissionId).Distinct().ToListAsync();

            return await _permissionsManager.QueryAsNoTracking.Where(x => permissionIds.Contains(x.Id)).Select(x => x.Code).ToListAsync();
        }

        public override IQueryable<PermissionRoleRelation> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override async Task ValidateOnCreateOrUpdate(PermissionRoleRelation entity)
        {
            await Task.CompletedTask;
        }

        public override Task ValidateOnDelete(PermissionRoleRelation entity)
        {
            throw new NotImplementedException();
        }
    }
}