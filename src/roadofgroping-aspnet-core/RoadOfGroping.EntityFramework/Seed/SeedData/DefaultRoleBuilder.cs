using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.Consts;
using RoadOfGroping.Core.Roles.Entity;
using RoadOfGroping.Core.Users.Entity;

namespace RoadOfGroping.EntityFramework.Seed.SeedData
{
    public class DefaultRoleBuilder
    {
        private readonly RoadOfGropingDbContext _context;

        public DefaultRoleBuilder(RoadOfGropingDbContext dbContext)
        {
            _context = dbContext;
        }

        public void Create()
        {
            CreateDefaultRole();
            CreateDefaultUserRole();
        }

        private void CreateDefaultRole()
        {
            // 创建 Default 用户。

            var defaultRole = _context.Roles.IgnoreQueryFilters()
                .FirstOrDefault(t => t.RoleName == RoadOfGropingConst.DefaultRoleName);
            if (defaultRole == null)
            {
                defaultRole = new Roles
                {
                    Id = RoadOfGropingConst.DefaultRoleId,
                    RoleName = RoadOfGropingConst.DefaultRoleName,
                    RoleCode = RoadOfGropingConst.DefaultRoleCode,
                    IsDeleted = false,
                };

                _context.Roles.Add(defaultRole);
                _context.SaveChanges();
            }
        }

        private void CreateDefaultUserRole()
        {
            // 创建 Default 用户。

            var defaultRole = _context.UserRoles.IgnoreQueryFilters()
                .FirstOrDefault(t => t.Id == RoadOfGropingConst.DefaultUserRoleId);
            if (defaultRole == null)
            {
                defaultRole = new UserRoles
                {
                    Id = RoadOfGropingConst.DefaultUserRoleId,
                    UserId = RoadOfGropingConst.DefaultUserId,
                    RoleId = RoadOfGropingConst.DefaultRoleId,
                    IsDeleted = false,
                };

                _context.UserRoles.Add(defaultRole);
                _context.SaveChanges();
            }
        }
    }
}