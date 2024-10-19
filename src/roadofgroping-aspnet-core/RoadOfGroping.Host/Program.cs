using RoadOfGroping.Host.Extensions;
using RoadOfGroping.Host.UnifyResult.Fiters;

var builder = WebApplication.CreateBuilder(args);
// �����ļ���ȡ
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

// ���HttpContextAccessor����
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers(c =>
{
    c.Filters.AddService<UnitOfWorkFilter>(99);
    //����ֵ������
    c.Filters.AddService<SucceededUnifyResultFilter>(100);
    //c.Filters.Add<ApiResultFilterAttribute>();
    //c.Filters.Add<ModelValidateActionFilterAttribute>();
});
// ���DbContext����
//builder.Services.UsingDatabaseServices(config);
// ���Autofac����ע��
builder.Host.UserAutoFac();

builder.AddCoreServices();
// ������־
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

builder.Services.AddRazorPages();

//// ע��EventBus����
//builder.Services.AddEventBusAndSubscribes(c =>
//{
//    c.Subscribe<TestDto, TestEventHandler>();
//    c.Subscribe<FileEventDto, FileEventHandler>();
//});

builder.AddUseCore();