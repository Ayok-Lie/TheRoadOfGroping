using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                    Roles = new List<string> { "Admin" }
                });

            builder.Entity<RoadOfGropingUsers>(c =>
            {
                c.Property(u => u.Roles).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            });

            return builder;
        }
    }
}