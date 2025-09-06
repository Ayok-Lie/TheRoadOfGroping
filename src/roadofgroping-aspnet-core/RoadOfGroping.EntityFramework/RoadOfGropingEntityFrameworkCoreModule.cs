using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.Enums;
using RoadOfGroping.EntityFramework.Seed;
using RoadOfGroping.Model;
using RoadOfGroping.Model.Extensions;
using RoadOfGroping.Model.Modules;
using RoadOfGroping.Repository;
using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.EntityFramework
{
    [DependOn(typeof(RoadOfGropingRepositoryModule))]
    public class RoadOfGropingEntityFrameworkCoreModule : BaseModule
    {
        public override void ConfigerService(ServiceConfigerContext context)
        {
            var configuration = context.GetConfiguration();
            var databaseType = configuration
                .GetSection("ConnectionStrings:DatabaseType")
                .Get<DatabaseType>();
            string? connectionString = string.Empty;
            connectionString = configuration.GetSection("ConnectionStrings:Default").Get<string>();
            context.Services.AddDbContext<RoadOfGropingDbContext>(option =>
            {
                switch (databaseType)
                {
                    case DatabaseType.SqlServer:
                        option.UseSqlServer(connectionString);
                        //option.AddInterceptors(new FrameworkInterceptor());
                        break;

                    case DatabaseType.MySql:
                        option.UseMySql(
                            connectionString,
                            new MySqlServerVersion(new Version(8, 0, 31))
                        );
                        break;

                    case DatabaseType.Sqlite:
                        option.UseSqlite(connectionString);
                        break;

                    case DatabaseType.Psotgre:
                        option.UseNpgsql(connectionString);
                        break;

                    default:
                        throw new Exception("不支持的数据库类型");
                }
            });
            context.Services.AddTransient(
                typeof(IEntityManager<RoadOfGropingDbContext>),
                typeof(EntityManager<RoadOfGropingDbContext>)
            );
        }

        public override void LaterInitApplication(InitApplicationContext context)
        {
            var entityManager = context.ServiceProvider.GetRequiredService<
                IEntityManager<RoadOfGropingDbContext>
            >();

            //添加种子数据
            entityManager.DbSeed(
                (dbcontext) =>
                {
                    SeedHelper.SeedDbData(dbcontext, context.ServiceProvider);
                }
            );
        }
    }
}
