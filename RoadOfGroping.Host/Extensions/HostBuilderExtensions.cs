using Autofac;
using Autofac.Extensions.DependencyInjection;
using FreeRedis;
using Newtonsoft.Json;
using RoadOfGroping.Utility.Autofac;
using RoadOfGroping.Utility.RedisModule;

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