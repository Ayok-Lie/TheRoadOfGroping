using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Roles.Entity.DomainService
{
    public interface IRoleManager : IAnotherDomainService<Roles, string>
    {
        Task<Roles> CreateAsync(Roles role);

        Task<Roles> UpdateAsync(Roles role);
    }
}