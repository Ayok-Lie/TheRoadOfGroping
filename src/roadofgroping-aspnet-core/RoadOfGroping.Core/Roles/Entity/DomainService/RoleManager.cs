using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Roles.Entity.DomainService
{
    public class RoleManager : AnotherDomainService<Roles, string>, IRoleManager
    {
        public RoleManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Roles> CreateAsync(Roles role)
        {
            return await Create(role);
        }

        public async Task<Roles> UpdateAsync(Roles role)
        {
            return await Update(role);
        }

        public override IQueryable<Roles> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }


        public override Task ValidateOnCreateOrUpdate(Roles entity)
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnDelete(Roles entity)
        {
            throw new NotImplementedException();
        }
    }
}