using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.EntityFramework.Extensions;

namespace RoadOfGroping.EntityFramework
{
    public static class RoadOfGropingEntityFrameworkCoreModule
    {
        public static void UsingDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseType = configuration.GetSection("ConnectionStrings:DatabaseType").Get<DatabaseType>();
            string? connectionString = string.Empty;
            connectionString = configuration.GetSection("ConnectionStrings:Default").Get<string>();
            services.AddDbContext<RoadOfGropingDbContext>(option =>
            {
                switch (databaseType)
                {
                    case DatabaseType.SqlServer:
                        option.UseSqlServer(connectionString);
                        break;

                    case DatabaseType.MySql:
                        option.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31)));
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
        }
    }
}