using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.Consts;

namespace RoadOfGroping.EntityFramework.Seed.SeedData
{
    public class DefaultUserBuilder
    {
        private readonly RoadOfGropingDbContext _context;

        public DefaultUserBuilder(RoadOfGropingDbContext dbContext)
        {
            _context = dbContext;
        }

        public void Create()
        {
            CreateDefaultUser();
        }

        private void CreateDefaultUser()
        {
            // 创建 Default 用户。

            var defaultUser = _context.Users.IgnoreQueryFilters()
                .FirstOrDefault(t => t.UserName == RoadOfGropingConst.DefaultUserName);
            if (defaultUser == null)
            {
                defaultUser = new Core.Users.Entity.Users
                {
                    Id = RoadOfGropingConst.DefaultUserId,
                    UserName = RoadOfGropingConst.DefaultUserName,
                    Password = RoadOfGropingConst.DefaultPassWord,
                    UserPhone = RoadOfGropingConst.DefaultUserPhone,
                    UserEmail = RoadOfGropingConst.DefaultUserEmail,
                    Avatar = RoadOfGropingConst.DefaulUserAvatar,
                    IsDeleted = false,
                };

                _context.Users.Add(defaultUser);
                _context.SaveChanges();
            }
        }
    }
}