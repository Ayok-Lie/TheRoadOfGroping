using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users
{
    public class UserManager : BasicDomainService<RoadOfGropingUsers, Guid>, IUserManager
    {
        public UserManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<RoadOfGropingUsers> Login(string username, string password)
        {
            var users = Query.ToList();
            var user = await Query.Where(a => a.UserName == username && a.PasswordHash == password).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException(username);
            }
            return user;
        }

        public override async Task ValidateOnCreateOrUpdate(RoadOfGropingUsers entity)
        {
            var count = await Query.Where(a => a.UserName == entity.UserName && a.Id != entity.Id).CountAsync();

            if (count > 0)
            {
                this.ThrowRepetError(entity.UserName);
            }
        }
    }
}