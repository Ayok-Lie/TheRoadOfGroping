using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RoadOfGroping.Application.Service.Handler;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.Common.JWTHelpers;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.Host.Extensions;
using RoadOfGroping.Host.Modules;
using RoadOfGroping.Model.Extensions;
using RoadOfGroping.Repository.Auditing;
using RoadOfGroping.Repository.DynamicWebAPI;
using RoadOfGroping.Repository.Extensions;
using RoadOfGroping.Repository.Middlewares;
using RoadOfGroping.Repository.UnitOfWorks;
using RoadOfGroping.Repository.UserSession;
using RoadOfGroping.Utility.ApiResult;
using RoadOfGroping.Utility.ErrorHandler;
using RoadOfGroping.Utility.EventBus.Extensions;
using RoadOfGroping.Utility.MessageCenter.SignalR;
using RoadOfGroping.Utility.Minio;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
// �����ļ���ȡ
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
// ����AppsettingHelper
builder.Services.AddSingleton(new AppsettingHelper(config));
// ���Controllers����
builder.Services.AddControllers();

#region ���Swagger�ĵ�����

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<AddResponseHeadersFilter>();
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.DocumentFilter<RemoveAppSuffixFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TheRoadOfGroping API",
        Version = "v1",
        Description = "Asp.Net Core6 WebApi����ʵս",
        Contact = new OpenApiContact()
        {
            Name = "Lie",
            Email = "88888888@qq.com",
            Url = new Uri("https://blog.csdn.net/ousetuhou?type=blog")
        }
    });

    var xmlPath = Path.Combine($"{basePath}", "ApiDoc.xml");
    options.IncludeXmlComments(xmlPath, true);
    //����AuthorizeȨ�ް�ť������ʽһ
    options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
    {
        Description = "���Ƿ�ʽһ(ֱ�����������������֤��Ϣ������Ҫ�ڿ�ͷ���Bearer) ",
        Name = "Authorization",        //jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,  //jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
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

    ////����AuthorizeȨ�ް�ť������ʽ��

    //options.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme()
    //{
    //    Description = "���Ƿ�ʽ��(JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�)",
    //    Name = "Authorization",//jwtĬ�ϵĲ�������
    //    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
    //    Type = SecuritySchemeType.ApiKey
    //});

    ////����AuthorizeȨ�ް�ť����Ĭ��
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

    //����һ��Scheme��ע�������IdҪ������AddSecurityDefinition�еĲ���nameһ��
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

#endregion ���Swagger�ĵ�����

// ���Autofac����ע��
builder.Host.UserAutoFac();

// ���DbContext����
builder.Services.AddDbContext<RoadOfGropingDbContext>(option =>
{
    option.UseSqlServer(config.GetConnectionString("Default"));
});

// ������־
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

#region ���JWT�����֤

var jwtTokenConfig = config.GetSection("JWT").Get<JwtModel>();
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "BearerCokkie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
    options.LogoutPath = "/Home/Index";
    options.Events = new CookieAuthenticationEvents
    {
        OnSigningOut = async context =>
        {
            context.Response.Cookies.Delete("access-token");
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.SecretKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
    options.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "�ܱ�Ǹ������Ȩ���ʸýӿ�" });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.WriteAsync(payload);
            return Task.CompletedTask;
        }
    };
});

#endregion ���JWT�����֤

builder.Services.AddRazorPages();
//ͳһ����ֵ
builder.Services.AddMvc(options => { })
.AddRazorPagesOptions((options) => { })
.AddRazorRuntimeCompilation()
.AddDynamicWebApi();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//// ���Ӧ�ó���ģ��
builder.Services.AddApplication<RoadOfGropingHostModule>();

// ע��ִ�����
builder.Services.UseRepository<RoadOfGropingDbContext>();

// ע��UnitOfWork����
builder.Services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork<RoadOfGropingDbContext>));
builder.Services.AddTransient<UnitOfWorkMiddleware>();

// ����CORS����
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddControllers(c =>
{
    c.Filters.Add<ApiResultFilterAttribute>();
});

//ע��Redis
builder.Services.UseRedis(config);

// ע��EventBus����
builder.Services.AddEventBusAndSubscribes(c =>
{
    c.Subscribe<TestDto, TestEventHandler>();
});
//ע��SignalR
builder.Services.AddSignalR();
//ע��Minio
builder.Services.AddMinio(config);
//ע��
builder.Services.AddTransient<IUserSession, CurrentUserSession>();
builder.Services.AddTransient<IAuditPropertySetter, AuditPropertySetter>();

var app = builder.Build();

// ����HTTP����ܵ�
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
        //options.RoutePrefix = String.Empty;
        //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        //options.DefaultModelExpandDepth(-1);
        //options.EnableDeepLinking(); //�����ӹ���
        options.DocExpansion(DocExpansion.None); //swagger�ĵ��Ƿ��
        options.IndexStream = () =>
        {
            var path = Path.Join(builder.Environment.WebRootPath, "pages", "swagger.html");
            return new FileInfo(path).OpenRead();
        };
    });
}

//app.UseStaticFiles();

//HTTPS�ض�����
//app.UseHttpsRedirection();

// ����CORS����
app.UseCors("DefaultCorsPolicy");

// ����쳣�����м��
app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

// ���������֤
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<TestChatHub>("/testHub");
// ���UnitOfWork�м��
app.UseMiddleware<UnitOfWorkMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    //endpoints.MapControllerRoute(
    //name: "default",
    //pattern: "{controller=Home}/{action=Privacy}/{id?}");
    endpoints.MapRazorPages();
});
//app.MapControllers();
app.Run();