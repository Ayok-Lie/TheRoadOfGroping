using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users.DomainService
{
    public class UserRolesManager : AnotherDomainService<UserRoles, string>, IUserRolesManager
    {
        public UserRolesManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task CreateAsync(UserRoles userRoles)
        {
            await Create(userRoles);
        }

        public async Task UpdateAsync(UserRoles userRoles)
        {
            await Update(userRoles);
        }

        public override IQueryable<UserRoles> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override async Task ValidateOnCreateOrUpdate(UserRoles entity)
        {
            await Task.CompletedTask;
        }

        public override Task ValidateOnDelete(UserRoles entity)
        {
            throw new NotImplementedException();
        }
    }
}