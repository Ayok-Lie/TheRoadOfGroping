using Hangfire;
using RoadOfGroping.Application.Service.Handler;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.Files.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ApiResult;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ErrorHandler;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Extensions;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.SignalR;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Minio;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.Host.Extensions;
using RoadOfGroping.Host.Modules;
using RoadOfGroping.Model.Extensions;
using RoadOfGroping.Repository.Auditing;
using RoadOfGroping.Repository.Extensions;
using RoadOfGroping.Repository.Middlewares;
using RoadOfGroping.Repository.UserSession;

var builder = WebApplication.CreateBuilder(args);
// 配置文件读取
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

// 配置AppsettingHelper
builder.Services.AddSingleton(new AppsettingHelper(config));

// 添加HttpContextAccessor服务
builder.Services.AddHttpContextAccessor();

// 添加Swagger文档
builder.AddSwaggerGen();

// 添加Autofac依赖注入
builder.Host.UserAutoFac();

// 添加DbContext服务
builder.Services.UsingDatabaseServices(config);

// 配置日志
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

// 添加JWT身份验证

builder.AddJwtConfig(config);

builder.Services.AddRazorPages();

builder.AddMvcConfig();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//// 添加应用程序模块
builder.Services.AddApplication<RoadOfGropingHostModule>();

// 注册仓储服务
builder.Services.UseRepository<RoadOfGropingDbContext>();

// 注册UnitOfWork服务

builder.AddUnitOfWork();

// 配置CORS策略

builder.AddCors();

builder.Services.AddControllers(c =>
{
    //返回值拦截器
    c.Filters.Add<ApiResultFilterAttribute>();
    c.Filters.Add<ModelValidateActionFilterAttribute>();
});

//注入Redis
builder.Services.UseRedis(config);

// 注册EventBus服务
builder.Services.AddEventBusAndSubscribes(c =>
{
    c.Subscribe<TestDto, TestEventHandler>();
    c.Subscribe<FileEventDto, FileEventHandler>();
});
//注入SignalR
builder.Services.AddSignalR();
//注入Minio
builder.Services.AddMinio(config);
//注入用户Session
builder.Services.AddTransient<IUserSession, CurrentUserSession>();
builder.Services.AddTransient<IAuditPropertySetter, AuditPropertySetter>();

#region 注入AutoMapper

builder.AddAutoMapper();

#endregion 注入AutoMapper

//配置Hangfire
builder.Services.ConfigureHangfireService();
var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(builder);
}

//app.UseStaticFiles();

//HTTPS重定向功能
//app.UseHttpsRedirection();

// 启用CORS策略
app.UseCors("DefaultCorsPolicy");

app.UseRouting();

// 添加异常处理中间件
app.UseMiddleware<ExceptionMiddleware>();
// 添加UnitOfWork中间件
app.UseMiddleware<UnitOfWorkMiddleware>();

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
//启用仪表盘

// 启用Hangfire仪表盘

app.UseHangfireDashboard();

app.Run();