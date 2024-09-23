using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.Users.Dtos;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Core.Users
{
    public class UserManager : BasicDomainService<RoadOfGropingUsers, Guid>, IUserManager
    {
        public UserManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<RoadOfGropingUsers> Login(UserInfo userInfo)
        {
            var users = Query.ToList();
            var user = await Query.Where(a => a.UserName == userInfo.UserName && a.PasswordHash == userInfo.Password).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException(userInfo.UserName + " is not a valid user");
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