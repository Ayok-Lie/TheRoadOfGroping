using System.Security.Cryptography;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FreeRedis;
using Hangfire;
using Hangfire.MySql;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RoadOfGroping.Application.Service.Mappers;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.Common.JWTHelpers;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Autofac;
using RoadOfGroping.Core.ZRoadOfGropingUtility.AutoMapper;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ErrorHandler;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.SignalR;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations;
using RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.EntityFramework.Extensions;
using RoadOfGroping.Host.UnifyResult.Fiters;
using RoadOfGroping.Model;
using RoadOfGroping.Repository.Auditing;
using RoadOfGroping.Repository.DynamicWebAPI;
using RoadOfGroping.Repository.Extensions;
using RoadOfGroping.Repository.Repository;
using RoadOfGroping.Repository.UnitOfWorks;
using RoadOfGroping.Repository.UserSession;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace RoadOfGroping.Host.Extensions
{
    /// <summary>
    /// 基础配置
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// 应用配置
        /// </summary>
        public static IConfiguration? configuration { get; private set; }

        public static IWebHostEnvironment Env { get; set; }

        /// <summary>
        /// 添加核心服务
        /// </summary>
        /// <param name="builder"></param>
        public static void AddCoreServices(this WebApplicationBuilder builder)
        {
            var context = new ServiceConfigerContext(builder.Services);
            configuration = context.Provider.GetRequiredService<IConfiguration>();
            Env = builder.Environment;
            // 配置AppsettingHelper
            builder.Services.AddSingleton(new AppsettingHelper(configuration));
            builder.Services.UseRepository();
            builder.Services.ConfigureHangfireService();
            builder.Services.ServicesMvc();
            builder.Services.AddSwaggerGen(builder);
            builder.Services.AddUnitOfWork();
            builder.Services.AddJwtConfig();
            builder.Services.AddCors();
            builder.Services.AddAutoMapper();
            builder.Services.UseRedis();
            builder.Services.AddSignalR();
            builder.Services.AddSession();
            builder.Services.AddMinio(configuration);
            builder.Services.AddFilters();
        }

        /// <summary>
        /// 管道服务配置
        /// </summary>
        /// <param name="builder"></param>
        public static void AddUseCore(this WebApplicationBuilder builder)
        {
            var app = builder.Build();
            app.UseSwaggerUI(builder);
            app.UseHttpsRedirection();
            // 启用CORS策略
            app.UseCors("DefaultCorsPolicy");

            app.UseRouting();

            // 添加异常处理中间件
            app.UseMiddleware<ExceptionMiddleware>();
            // 添加UnitOfWork中间件
            //app.UseMiddleware<UnitOfWorkMiddleware>();

            // 启用身份验证
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<TestChatHub>("/testHub");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllerRoute(
                //name: "default",
                //pattern: "{controller=Home}/{action=Privacy}/{id?}");
                endpoints.MapRazorPages();
            });
            //app.MapControllers();

            // 启用Hangfire仪表盘

            app.UseHangfireDashboard();
            app.UseDeveloperExceptionPage();

            app.Run();
        }

        /// <summary>
        /// AutoFac 配置
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static void UserAutoFac(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseServiceProviderFactory(
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
        /// 仓储服务
        /// </summary>
        public static void UseRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAnotherBaseRepository<,>), typeof(AnotherBaseRepository<,>));
            // 添加应用程序模块
            //builder.Services.AddApplication<RoadOfGropingHostModule>();

            // 注册仓储服务
            services.UseRepository<RoadOfGropingDbContext>();
        }

        /// <summary>
        /// 注册Hangfire
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
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
        /// 动态WebAPI 配置
        /// </summary>
        /// <returns></returns>
        public static void ServicesMvc(this IServiceCollection services)
        {
            //成功规范接口
            services.AddTransient<SucceededUnifyResultFilter>();
            services.AddMvc(options =>
            {
            })
                .AddRazorPagesOptions((options) => { })
                .AddRazorRuntimeCompilation()
                .AddDynamicWebApi();
        }

        /// <summary>
        /// Swagger 配置
        /// </summary>
        /// <returns></returns>
        public static void AddSwaggerGen(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                //options.DocumentFilter<RemoveAppSuffixFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TheRoadOfGroping API",
                    Version = "v1",
                    Description = "Asp.Net Core6 WebApi开发实战",
                    Contact = new OpenApiContact()
                    {
                        Name = "Lie",
                        Email = "88888888@qq.com",
                        Url = new Uri("https://blog.csdn.net/ousetuhou?type=blog")
                    }
                });
                var binXmlFiles =
                new DirectoryInfo(Path.Join(builder?.Environment.WebRootPath, "ApiDocs"))
                .GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                foreach (var filePath in binXmlFiles.Select(item => item.FullName))
                {
                    options.IncludeXmlComments(filePath, true);
                }
                //开启Authorize权限按钮——方式一
                options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
                {
                    Description = "这是方式一(直接在输入框中输入认证信息，不需要在开头添加Bearer) ",
                    Name = "Authorization",        //jwt默认的参数名称
                    In = ParameterLocation.Header,  //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "JWTBearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                ////开启Authorize权限按钮——方式二

                //options.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme()
                //{
                //    Description = "这是方式二(JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）)",
                //    Name = "Authorization",//jwt默认的参数名称
                //    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                //    Type = SecuritySchemeType.ApiKey
                //});

                ////开启Authorize权限按钮——默认
                //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            },Scheme = "oauth2",Name = "Bearer",In=ParameterLocation.Header,
                //        },new List<string>()
                //    }
                //});

                //声明一个Scheme，注意下面的Id要和上面AddSecurityDefinition中的参数name一致
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { scheme, Array.Empty<string>() }
                });
            });
        }

        /// <summary>
        /// 注册UnitOfWork服务
        /// </summary>
        /// <returns></returns>
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork<RoadOfGropingDbContext>));
            //services.AddTransient<UnitOfWorkMiddleware>();
        }

        /// <summary>
        /// JWT 配置
        /// </summary>
        /// <returns></returns>
        public static void AddJwtConfig(this IServiceCollection services)
        {
            services.AddTransient<DataSeeds, DataSeeds>();
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();
            //services.AddTransient<IAuthorizationHandler, PermissionUrlHandler>();

            var rsaSecurityPrivateKeyString = File.ReadAllText(Path.Combine(Env.ContentRootPath, "Rsa", "key.private.json"));
            var rsaSecurityPublicKeyString = File.ReadAllText(Path.Combine(Env.ContentRootPath, "Rsa", "key.public.json"));
            RsaSecurityKey rsaSecurityPrivateKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPrivateKeyString));
            RsaSecurityKey rsaSecurityPublicKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPublicKeyString));
            services.AddSingleton(sp => new SigningCredentials(rsaSecurityPrivateKey, SecurityAlgorithms.RsaSha256Signature));

            services.AddScoped<AppJwtBearerEvents>();
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Name));
            var jwtTokenConfig = configuration?.GetSection("JWT").Get<JwtOptions>();

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "auth";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = false;
                options.LogoutPath = "/Home/Index";
                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningOut = async context =>
                    {
                        context.Response.Cookies.Delete("Authorization");
                        await Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = rsaSecurityPublicKey,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
                options.EventsType = typeof(AppJwtBearerEvents);
            });
        }

        /// <summary>
        /// CORS策略
        /// </summary>
        /// <returns></returns>
        public static void AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        /// <summary>
        /// Redis 注册
        /// </summary>
        /// <returns></returns>
        public static void UseRedis(this IServiceCollection services)
        {
            // 获取缓存相关配置
            var cacheConfig = configuration?.GetSection("CacheConfig").Get<RedisCacheOptions>();

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
                // 注册内存缓存服务
                services.AddMemoryCache();

                services.AddSingleton<ICacheTool, MemoryCacheTool>();

                // 注册分布式内存缓存服务
                services.AddDistributedMemoryCache();
            }
        }

        /// <summary>
        /// 注册AutoMapper
        /// </summary>
        /// <returns></returns>
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton(MapperHepler.CreateMappings);
            services.AddSingleton<IMapper>(provider => provider.GetRequiredService<Mapper>());
            services.Configure<AutoMapperOptions>(options =>
            {
                options.Configurators.Add(ctx =>
                {
                    AutoMappers.CreateMappings(ctx.MapperConfiguration);
                });
            });
        }

        /// <summary>
        /// 用户Session
        /// </summary>
        /// <param name="services"></param>
        public static void AddSession(this IServiceCollection services)
        {
            //注入用户Session
            services.AddTransient<IUserSession, CurrentUserSession>();
            services.AddTransient<IAuditPropertySetter, AuditPropertySetter>();
        }

        /// <summary>
        /// 使用 Hangfire Storage
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IGlobalConfiguration UseHangfireStorage(this IGlobalConfiguration configuration)
        {
            var databaseType = AppsettingHelper.AppOption<DatabaseType>("ConnectionStrings:DatabaseType");
            string? connectionString = string.Empty;
            connectionString = AppsettingHelper.AppOption<string>("ConnectionStrings:Default");
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    configuration.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        SchemaName = "RoadOfGroping_HangFire_",
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5), // 批处理作业的最大超时时间为 5 分钟
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // 作业的可见性超时时间为 5 分钟
                        QueuePollInterval = TimeSpan.FromSeconds(5), // 检查作业队列的间隔时间为 5 秒
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),//- 作业到期检查间隔（管理过期记录）。默认值为1小时。
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),//- 聚合计数器的间隔。默认为5分钟。
                        DashboardJobListLimit = 5000,//- 仪表板作业列表限制。默认值为50000。
                        TransactionTimeout = TimeSpan.FromMinutes(1),//- 交易超时。默认为1分钟。
                        UseRecommendedIsolationLevel = true, // 使用推荐的事务隔离级别
                        DisableGlobalLocks = true // 禁用全局锁定机制
                    });
                    break;

                case DatabaseType.MySql:
                    configuration.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions()
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "RoadOfGroping_HangFire_"
                    }));
                    break;

                case DatabaseType.Psotgre:
                    break;

                case DatabaseType.Sqlite:
                    break;

                default:
                    throw new Exception("不支持的数据库类型");
            }
            return configuration;
        }

        /// <summary>
        /// 注册全局拦截器
        /// </summary>
        public static void AddFilters(this IServiceCollection services)
        {
            // 注册全局拦截器
            services.AddControllersWithViews(x =>
            {
                //全局返回，统一返回格式
                //x.Filters.Add<ResFilter>();

                //全局日志，报错
                //x.Filters.Add<LogAttribute>();

                //全局身份验证
                //x.Filters.Add<TokenAttribute>();
            });
        }

        /// <summary>
        /// 配置swaggerUI
        /// </summary>
        /// <returns></returns>
        public static void UseSwaggerUI(this WebApplication app, WebApplicationBuilder builder)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
                    //options.RoutePrefix = String.Empty;
                    //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                    //options.DefaultModelExpandDepth(-1);
                    //options.EnableDeepLinking(); //深链接功能
                    options.DocExpansion(DocExpansion.None); //swagger文档是否打开
                    options.IndexStream = () =>
                    {
                        var path = Path.Join(builder.Environment!.WebRootPath, "pages", "swagger.html");
                        return new FileInfo(path).OpenRead();
                    };
                });
            }
        }
    }
}