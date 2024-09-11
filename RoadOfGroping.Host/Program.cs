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
// �����ļ���ȡ
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

// ����AppsettingHelper
builder.Services.AddSingleton(new AppsettingHelper(config));

// ���HttpContextAccessor����
builder.Services.AddHttpContextAccessor();

// ���Swagger�ĵ�
builder.AddSwaggerGen();

// ���Autofac����ע��
builder.Host.UserAutoFac();

// ���DbContext����
builder.Services.UsingDatabaseServices(config);

// ������־
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

// ���JWT�����֤

builder.AddJwtConfig(config);

builder.Services.AddRazorPages();

builder.AddMvcConfig();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//// ���Ӧ�ó���ģ��
builder.Services.AddApplication<RoadOfGropingHostModule>();

// ע��ִ�����
builder.Services.UseRepository<RoadOfGropingDbContext>();

// ע��UnitOfWork����

builder.AddUnitOfWork();

// ����CORS����

builder.AddCors();

builder.Services.AddControllers(c =>
{
    //����ֵ������
    c.Filters.Add<ApiResultFilterAttribute>();
    c.Filters.Add<ModelValidateActionFilterAttribute>();
});

//ע��Redis
builder.Services.UseRedis(config);

// ע��EventBus����
builder.Services.AddEventBusAndSubscribes(c =>
{
    c.Subscribe<TestDto, TestEventHandler>();
    c.Subscribe<FileEventDto, FileEventHandler>();
});
//ע��SignalR
builder.Services.AddSignalR();
//ע��Minio
builder.Services.AddMinio(config);
//ע���û�Session
builder.Services.AddTransient<IUserSession, CurrentUserSession>();
builder.Services.AddTransient<IAuditPropertySetter, AuditPropertySetter>();

#region ע��AutoMapper

builder.AddAutoMapper();

#endregion ע��AutoMapper

//����Hangfire
builder.Services.ConfigureHangfireService();
var app = builder.Build();

// ����HTTP����ܵ�
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(builder);
}

//app.UseStaticFiles();

//HTTPS�ض�����
//app.UseHttpsRedirection();

// ����CORS����
app.UseCors("DefaultCorsPolicy");

app.UseRouting();

// ����쳣�����м��
app.UseMiddleware<ExceptionMiddleware>();
// ���UnitOfWork�м��
app.UseMiddleware<UnitOfWorkMiddleware>();

// ���������֤
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
//�����Ǳ���

// ����Hangfire�Ǳ���

app.UseHangfireDashboard();

app.Run();