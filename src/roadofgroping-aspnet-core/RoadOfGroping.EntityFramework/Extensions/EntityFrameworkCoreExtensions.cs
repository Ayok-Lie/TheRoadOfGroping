using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.Roles.Entity;
using RoadOfGroping.Core.Users.Entity;

namespace RoadOfGroping.EntityFramework.Extensions
{
    public static class EntityFrameworkCoreExtensions
    {
        public static ModelBuilder ConfigureModel(this ModelBuilder builder)
        {
            #region 用户

            builder.Entity<Users>().HasMany<UserRoles>().WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

            #endregion 用户

            #region 角色

            builder.Entity<Roles>().HasMany<UserRoles>().WithOne().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Roles>(c =>
            {
                c.HasIndex(o => o.RoleName).IsUnique().HasFilter("IsDeleted = 0");
                c.HasIndex(o => o.RoleCode).IsUnique().HasFilter("IsDeleted = 0");
            });

            #endregion 角色

            //builder.Entity<Users>(c =>
            //{
            //    c.Property(u => u.Roles).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            //});

            return builder;
        }
    }
}