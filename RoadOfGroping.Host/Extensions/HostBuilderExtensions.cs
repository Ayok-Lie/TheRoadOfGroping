using Autofac;
using Autofac.Extensions.DependencyInjection;
using FreeRedis;
using Hangfire;
using Hangfire.SqlServer;
using Newtonsoft.Json;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Autofac;
using RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule;

namespace RoadOfGroping.Host.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UserAutoFac(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseServiceProviderFactory(
                new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(builder =>
                    {
                        builder.RegisterModule(new AutofacModule());
                        builder.RegisterBuildCallback(scope =>
                        {
                            IOCManager.Current = (IContainer)scope;
                        });
                    });
        }

        /// <summary>
        /// Redis 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection UseRedis(this IServiceCollection services, IConfiguration configuration)
        {
            // 获取缓存相关配置
            var cacheConfig = configuration.GetSection("CacheConfig").Get<RedisCacheOptions>();

            // 注册内存缓存服务
            services.AddMemoryCache();

            // 判断是否启用Redis缓存
            if (cacheConfig.EnableRedis)
            {
                // 创建Redis连接字符串构建器
                var connectionStringBuilder = new ConnectionStringBuilder()
                {
                    Host = cacheConfig.Redis.Host, // Redis主机地址
                    Password = cacheConfig.Redis.Password, // Redis密码
                    Database = cacheConfig.Redis.Database, // 选择的数据库
                    Ssl = cacheConfig.Redis.Ssl // 是否启用SSL
                };

                // 创建Redis客户端实例
                var redis = new RedisClient(connectionStringBuilder)
                {
                    Serialize = JsonConvert.SerializeObject, // 序列化函数
                    Deserialize = JsonConvert.DeserializeObject, // 反序列化函数
                };

                // 注册Redis客户端实例为单例服务
                services.AddSingleton(redis);
                services.AddSingleton<IRedisClient>(redis);

                // 注册Redis缓存工具为单例服务
                services.AddSingleton<ICacheTool, RedisCacheTool>();

                // 配置客户端缓存选项
                var options = new ClientSideCachingOptions();
                redis.UseClientSideCaching(options);
            }
            else
            {
                // 注册内存缓存工具为单例服务
                services.AddSingleton<ICacheTool, MemoryCacheTool>();

                // 注册分布式内存缓存服务
                services.AddDistributedMemoryCache();
            }

            return services;
        }

        /// <summary>
        /// 注册Hangfire
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureHangfireService(this IServiceCollection services,
            Action<BackgroundJobServerOptions> optionsAction = null)
        {
            var enable = AppsettingHelper.Get("HangFire:HangfireEnabled");
            if (enable.Equals("false")) return;

            var options = new BackgroundJobServerOptions()
            {
                ShutdownTimeout = TimeSpan.FromMinutes(30),
                Queues = new string[] { "default", "jobs" }, //队列名称，只能为小写
                WorkerCount = 3, //Environment.ProcessorCount * 5, //并发任务数 Math.Max(Environment.ProcessorCount, 20)
                ServerName = "fantasy.hangfire",
            };
            optionsAction?.Invoke(options);
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)//向前兼容
                .UseSimpleAssemblyNameTypeSerializer()//使用简单的程序集名称类型序列化器
                .UseRecommendedSerializerSettings()// 使用推荐的序列化器设置
                .UseHangfireStorage()
            ).AddHangfireServer(optionsAction: c => c = options);
        }

        /// <summary>
        /// 使用 Hangfire Storage
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IGlobalConfiguration UseHangfireStorage(this IGlobalConfiguration configuration)
        {
            string connectionString = string.Empty;
            switch ("SqlServer")
            {
                case "SqlServer":
                    connectionString = AppsettingHelper.AppOption<string>("ConnectionStrings:Default");
                    configuration.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        SchemaName = "RoadOfGroping_HangFire_",
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5), // 批处理作业的最大超时时间为 5 分钟
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // 作业的可见性超时时间为 5 分钟
                        QueuePollInterval = TimeSpan.FromSeconds(5), // 检查作业队列的间隔时间为 5 秒
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),//- 作业到期检查间隔（管理过期记录）。默认值为1小时。
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),//- 聚合计数器的间隔。默认为5分钟。
                        //DashboardJobListLimit=5000,//- 仪表板作业列表限制。默认值为50000。
                        TransactionTimeout = TimeSpan.FromMinutes(1),//- 交易超时。默认为1分钟。
                        UseRecommendedIsolationLevel = true, // 使用推荐的事务隔离级别
                        DisableGlobalLocks = true // 禁用全局锁定机制
                    });
                    break;
                //case "MySql":
                //    connectionString = AppSettings.AppOption<string>("App:ConnectionString:Mysql");
                //    configuration.UseMysqlStorage(connectionString);
                //    break;
                default:
                    throw new Exception("不支持的数据库类型");
            }
            return configuration;
        }

        /// <summary>
        /// 使用Oracle的Hangfire Storage
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        //public static IGlobalConfiguration UseMysqlStorage(this IGlobalConfiguration configuration, string connectionString)
        //{
        //    var storage = new MySqlStorage(connectionString, new MySqlStorageOptions()
        //    {
        //        QueuePollInterval = TimeSpan.FromSeconds(15),
        //        JobExpirationCheckInterval = TimeSpan.FromHours(1),
        //        CountersAggregateInterval = TimeSpan.FromMinutes(5),
        //        PrepareSchemaIfNecessary = true,
        //        DashboardJobListLimit = 50000,
        //        TransactionTimeout = TimeSpan.FromMinutes(1),
        //        TablesPrefix = "Z_HangFire_"
        //    });

        //    configuration.UseStorage(storage);

        //    return configuration;
        //}
    }

    //public class AutofacModule : Autofac.Module
    //{
    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        //builder.RegisterType<AutoFacAop>();
    //        //程序集注入业务服务
    //        var IAppServices = Assembly.Load("RoadOfGroping.Core");
    //        var AppServices = Assembly.Load("RoadOfGroping.Core");
    //        //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
    //        builder.RegisterAssemblyTypes(AppServices)
    //          .Where(t => t.Name.EndsWith("Service"))
    //          .Where(t => typeof(IDefaultDomainService).IsAssignableFrom(t))
    //          //.Where(t => t.GetCustomAttribute<AutoFacAop>() != null)判断是否含有特性
    //          //.PublicOnly()//只要public访问权限的
    //          .Where(t => t.IsClass)
    //          //.PropertiesAutowired()
    //          .AsImplementedInterfaces() //方法会将每个选定的类注册为它们实现的所有接口
    //          .InstancePerDependency();
    //        //builder.Register(c => new TimeNotificationService(c.Resolve<ILogger<TimeNotificationService>>()))
    //        //    .As<ITimeNotificationService>()
    //        //    .InstancePerLifetimeScope();
    //    }
    //}
}