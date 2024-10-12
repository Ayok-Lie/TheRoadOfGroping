using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users.DomainService
{
    public class UserManager : BasicDomainService<Entity.Users, string>, IUserManager
    {
        private readonly IUserRolesManager _userRolesManager;

        public UserManager(IServiceProvider serviceProvider, IUserRolesManager userRolesManager) : base(serviceProvider)
        {
            _userRolesManager = userRolesManager;
        }

        public override async Task ValidateOnCreateOrUpdate(Entity.Users entity)
        {
            var count = await Query.Where(a => a.UserName == entity.UserName && a.Id != entity.Id).CountAsync();

            if (count > 0)
            {
                ThrowRepetError(entity.UserName);
            }
        }
    }
}