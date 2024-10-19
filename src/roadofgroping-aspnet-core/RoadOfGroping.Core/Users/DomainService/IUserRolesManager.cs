using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users.DomainService
{
    public interface IUserRolesManager : IAnotherDomainService<UserRoles, string>
    {
        Task CreateAsync(UserRoles userRoles);

        Task UpdateAsync(UserRoles userRoles);

        Task<List<string>> GetUserRoleIdsAsync(string userId);
    }
}