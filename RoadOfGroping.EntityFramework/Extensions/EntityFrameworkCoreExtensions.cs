using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Core.Users.Entity;

namespace RoadOfGroping.EntityFramework.Extensions
{
    public static class EntityFrameworkCoreExtensions
    {
        public static ModelBuilder ConfigureModel(this ModelBuilder builder)
        {
            builder.Entity<RoadOfGropingUsers>().HasData(
                new RoadOfGropingUsers
                {
                    Id = Guid.Parse("45D6422E-0EBB-45DB-DC2A-08DC86A36122"),
                    UserName = "admin",
                    PasswordHash = "bb123456",
                    UserPhone = "8888888888",
                    UserEmail = "admin@localhost",
                    IsDeleted = false,
                    Role = "Admin"
                });

            return builder;
        }
    }
}