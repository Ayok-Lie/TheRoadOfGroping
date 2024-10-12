using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Permissions.DomainService
{
    public class PermissionRoleRelationManager : AnotherDomainService<PermissionRoleRelation, Guid>, IPermissionRoleRelationManager
    {
        public PermissionRoleRelationManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<PermissionRoleRelation> CreateAsync(PermissionRoleRelation permissionRole)
        {
            return await Create(permissionRole);
        }

        public async Task<PermissionRoleRelation> UpdateAsync(PermissionRoleRelation permissionRole)
        {
            return await Update(permissionRole);
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