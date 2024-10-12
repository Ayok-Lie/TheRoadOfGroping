using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Permissions.DomainService
{
    public interface IPermissionsManager : IAnotherDomainService<PermissionOriginal, Guid>
    {
        IQueryable<PermissionOriginal> GetAll();
    }
}