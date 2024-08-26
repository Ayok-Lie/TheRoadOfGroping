using System.Text;
using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RoadOfGroping.Common.Helper;
using RoadOfGroping.Common.JWTHelpers;
using RoadOfGroping.EntityFramework;
using RoadOfGroping.Host.Extensions;

//using RoadOfGroping.EntityFramework.Extensions;
using RoadOfGroping.Host.Modules;
using RoadOfGroping.Model.Extensions;
using RoadOfGroping.Repository.Extensions;
using RoadOfGroping.Repository.Middlewares;
using RoadOfGroping.Repository.UnitOfWorks;
using RoadOfGroping.Utility.ApiResult;
using RoadOfGroping.Utility.ErrorHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var basePath = AppContext.BaseDirectory;
var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

//builder.Services.Configure<IConfiguration>(config);
builder.Services.AddSingleton(new AppsettingHelper(config));
//在AddControllers()之前添加以上两句
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region 添加Swagger注释

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TheRoadOfGroping API",
        Version = "v1",
        Description = "Asp.Net Core6 WebApi开发实战",  //描述信息
        Contact = new OpenApiContact()                //开发者信息
        {
            Name = "Lie",               //开发者姓名
            Email = "88888888@qq.com",    //email地址
            Url = new Uri("https://blog.csdn.net/ousetuhou?type=blog") //作者的主页网站
        }
    });

    var xmlPath = Path.Combine($"{basePath}/ApiDoc", "ApiDoc.xml"); // 这里的文件，需要了自己手动创建，并将属性改为 “始终复制”
    options.IncludeXmlComments(xmlPath, true);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    //开启Authorize权限按钮——方式一
    options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
    {
        Description = "这是方式一(直接在输入框中输入认证信息，不需要在开头添加Bearer) ",
        Name = "Authorization",        //jwt默认的参数名称
        In = ParameterLocation.Header,  //jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

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
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference()
        {
            Id = "JWTBearer",  //这个名字与上面的一样
            Type = ReferenceType.SecurityScheme
        }
    };
    //注册全局认证（所有的接口都可以使用认证）
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

#endregion 添加Swagger注释

#region 添加Autofac

builder.Host.UserAutoFac();

#endregion 添加Autofac

builder.Services.AddDbContext<RoadOfGropingDbContext>(option =>
{
    option.UseSqlServer(config.GetConnectionString("Default"));
});

builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

#region 添加身份验证JWT

//builder.Services.AddSingleton<
//    IAuthorizationMiddlewareResultHandler, RoadOfGropingAuthorizationMiddleware>();
//获取jwt配置项
var jwtTokenConfig = builder.Configuration.GetSection("JWT").Get<JwtModel>();

//添加jwt验证：
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //认证模式
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    //质询模式
})
.AddCookie(options =>
{
    //cokkie名称
    options.Cookie.Name = "BearerCokkie";
    //cokkie过期时间
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //cokkie启用滑动过期时间
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
        ValidateIssuerSigningKey = true, //是否验证SecurityKey
        ValidAudience = jwtTokenConfig.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.SecretKey)),
        ClockSkew = TimeSpan.FromMinutes(1)  //对token过期时间验证的允许时间
    };
    //如果jwt过期，在返回的header中加入Token-Expired字段为true，前端在获取返回header时判断
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
        //此处为权限验证失败后触发的事件
        OnChallenge = context =>
        {
            //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
            context.HandleResponse();

            //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
            var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "很抱歉，您无权访问该接口" });
            //自定义返回的数据类型
            context.Response.ContentType = "application/json";
            //自定义返回状态码，默认为401
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //输出Json数据结果
            context.Response.WriteAsync(payload);
            return Task.CompletedTask;
        }
    };
});

#endregion 添加身份验证JWT

#region 统一返回值

builder.Services.AddMvc(options =>
{
    options.Filters.Add<ApiResultFilterAttribute>();
});

#endregion 统一返回值

builder.Services.AddApplication<RoadOfGropingHostModule>();

//注册仓储服务
builder.Services.UseRepository<RoadOfGropingDbContext>();

builder.Services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork<RoadOfGropingDbContext>));
builder.Services.AddTransient<UnitOfWorkMiddleware>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
        c.RoutePrefix = String.Empty;
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DefaultModelExpandDepth(-1);
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

#region 启用身份验证

app.UseAuthentication();//你是谁在前
app.UseAuthorization();//能干什么在后

#endregion 启用身份验证

app.UseMiddleware<UnitOfWorkMiddleware>();

app.MapControllers();

app.Run();