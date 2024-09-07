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
// 配置文件读取
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
// 配置AppsettingHelper
builder.Services.AddSingleton(new AppsettingHelper(config));
// 添加Controllers服务
builder.Services.AddControllers();

#region 添加Swagger文档服务

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
        Description = "Asp.Net Core6 WebApi开发实战",
        Contact = new OpenApiContact()
        {
            Name = "Lie",
            Email = "88888888@qq.com",
            Url = new Uri("https://blog.csdn.net/ousetuhou?type=blog")
        }
    });

    var xmlPath = Path.Combine($"{basePath}", "ApiDoc.xml");
    options.IncludeXmlComments(xmlPath, true);
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

#endregion 添加Swagger文档服务

// 添加Autofac依赖注入
builder.Host.UserAutoFac();

// 添加DbContext服务
builder.Services.AddDbContext<RoadOfGropingDbContext>(option =>
{
    option.UseSqlServer(config.GetConnectionString("Default"));
});

// 配置日志
builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

#region 添加JWT身份验证

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
            var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "很抱歉，您无权访问该接口" });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.WriteAsync(payload);
            return Task.CompletedTask;
        }
    };
});

#endregion 添加JWT身份验证

builder.Services.AddRazorPages();
//统一返回值
builder.Services.AddMvc(options => { })
.AddRazorPagesOptions((options) => { })
.AddRazorRuntimeCompilation()
.AddDynamicWebApi();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//// 添加应用程序模块
builder.Services.AddApplication<RoadOfGropingHostModule>();

// 注册仓储服务
builder.Services.UseRepository<RoadOfGropingDbContext>();

// 注册UnitOfWork服务
builder.Services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork<RoadOfGropingDbContext>));
builder.Services.AddTransient<UnitOfWorkMiddleware>();

// 配置CORS策略
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

//注入Redis
builder.Services.UseRedis(config);

// 注册EventBus服务
builder.Services.AddEventBusAndSubscribes(c =>
{
    c.Subscribe<TestDto, TestEventHandler>();
});
//注入SignalR
builder.Services.AddSignalR();
//注入Minio
builder.Services.AddMinio(config);
//注入
builder.Services.AddTransient<IUserSession, CurrentUserSession>();
builder.Services.AddTransient<IAuditPropertySetter, AuditPropertySetter>();

var app = builder.Build();

// 配置HTTP请求管道
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
            var path = Path.Join(builder.Environment.WebRootPath, "pages", "swagger.html");
            return new FileInfo(path).OpenRead();
        };
    });
}

//app.UseStaticFiles();

//HTTPS重定向功能
//app.UseHttpsRedirection();

// 启用CORS策略
app.UseCors("DefaultCorsPolicy");

// 添加异常处理中间件
app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

// 启用身份验证
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<TestChatHub>("/testHub");
// 添加UnitOfWork中间件
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