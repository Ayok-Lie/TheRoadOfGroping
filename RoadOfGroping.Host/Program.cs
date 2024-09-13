using RoadOfGroping.Application.Service.Handler;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.Files.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ApiResult;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ErrorHandler;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Extensions;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.Host.Extensions;

var builder = WebApplication.CreateBuilder(args);
// 配置文件读取
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

// 添加HttpContextAccessor服务
builder.Services.AddHttpContextAccessor();
// 添加DbContext服务
builder.Services.UsingDatabaseServices(config);
// 添加Autofac依赖注入
builder.Host.UserAutoFac();

builder.AddCoreServices();
// 配置日志
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

builder.Services.AddRazorPages();

builder.Services.AddControllers(c =>
{
    //返回值拦截器
    c.Filters.Add<ApiResultFilterAttribute>();
    c.Filters.Add<ModelValidateActionFilterAttribute>();
});

// 注册EventBus服务
builder.Services.AddEventBusAndSubscribes(c =>
{
    c.Subscribe<TestDto, TestEventHandler>();
    c.Subscribe<FileEventDto, FileEventHandler>();
});

builder.AddUseCore();