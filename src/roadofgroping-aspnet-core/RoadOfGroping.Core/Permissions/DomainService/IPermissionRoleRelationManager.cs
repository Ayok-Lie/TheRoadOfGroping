using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Permissions.DomainService
{
    public interface IPermissionRoleRelationManager : IAnotherDomainService<PermissionRoleRelation, Guid>
    {
        Task<PermissionRoleRelation> CreateAsync(PermissionRoleRelation permissionRole);

        Task<PermissionRoleRelation> UpdateAsync(PermissionRoleRelation permissionRole);

        Task<List<string>> GetUserAllPermissionsAsync(List<string> roleIds);
    }
}