using RoadOfGroping.Application.Service.Handler;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.Files.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ApiResult;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ErrorHandler;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Extensions;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.Host.Extensions;

var builder = WebApplication.CreateBuilder(args);
// �����ļ���ȡ
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

// ���HttpContextAccessor����
builder.Services.AddHttpContextAccessor();
// ���DbContext����
builder.Services.UsingDatabaseServices(config);
// ���Autofac����ע��
builder.Host.UserAutoFac();

builder.AddCoreServices();
// ������־
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

builder.Services.AddRazorPages();

builder.Services.AddControllers(c =>
{
    //����ֵ������
    c.Filters.Add<ApiResultFilterAttribute>();
    c.Filters.Add<ModelValidateActionFilterAttribute>();
});

// ע��EventBus����
builder.Services.AddEventBusAndSubscribes(c =>
{
    c.Subscribe<TestDto, TestEventHandler>();
    c.Subscribe<FileEventDto, FileEventHandler>();
});

builder.AddUseCore();