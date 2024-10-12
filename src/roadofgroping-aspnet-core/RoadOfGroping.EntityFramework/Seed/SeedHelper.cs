using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.EntityFramework.Seed.SeedData;
using RoadOfGroping.Repository.UnitOfWorks;

namespace RoadOfGroping.EntityFramework.Seed
{
    public static class SeedHelper
    {
        public static void SeedDbData(RoadOfGropingDbContext dbContext, IServiceProvider serviceProvider)
        {
            var dbtype = AppsettingHelper.AppOption<string>("ConnectionStrings:DatabaseType")!;
            switch (dbtype.ToLower())
            {
                case "sqlserver":
                    var isConnect = dbContext.Database.CanConnect();
                    if (!isConnect) throw new Exception($"数据库连接错误,连接字符串:'{dbContext.Database.GetConnectionString()}'");
                    break;

                case "mysql":
                    break;

                default:
                    throw new Exception("不支持的数据库类型");
            }

            WithDbContext(serviceProvider, dbContext, SeedDbData);
        }

        public static void SeedDbData(RoadOfGropingDbContext context)
        {
            new DefaultPermissionBuilder(context).Create();
            new DefaultUserBuilder(context).Create();
            new DefaultRoleBuilder(context).Create();
        }

        private static void WithDbContext<TDbContext>(IServiceProvider serviceProvider, TDbContext dbContext, Action<TDbContext> contextAction)
                where TDbContext : DbContext
        {
            using (var uowManager = serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                using (var uow = uowManager.BeginTransactionAsync().Result)
                {
                    try
                    {
                        contextAction(dbContext);

                        uow.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        uow.RollbackAsync();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}