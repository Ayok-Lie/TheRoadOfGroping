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
//��AddControllers()֮ǰ�����������
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region ���Swaggerע��

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TheRoadOfGroping API",
        Version = "v1",
        Description = "Asp.Net Core6 WebApi����ʵս",  //������Ϣ
        Contact = new OpenApiContact()                //��������Ϣ
        {
            Name = "Lie",               //����������
            Email = "88888888@qq.com",    //email��ַ
            Url = new Uri("https://blog.csdn.net/ousetuhou?type=blog") //���ߵ���ҳ��վ
        }
    });

    var xmlPath = Path.Combine($"{basePath}/ApiDoc", "ApiDoc.xml"); // ������ļ�����Ҫ���Լ��ֶ��������������Ը�Ϊ ��ʼ�ո��ơ�
    options.IncludeXmlComments(xmlPath, true);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ���·�����Bearer {token} ���ɣ�ע������֮���пո�",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    //����AuthorizeȨ�ް�ť������ʽһ
    options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
    {
        Description = "���Ƿ�ʽһ(ֱ�����������������֤��Ϣ������Ҫ�ڿ�ͷ���Bearer) ",
        Name = "Authorization",        //jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,  //jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

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
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference()
        {
            Id = "JWTBearer",  //��������������һ��
            Type = ReferenceType.SecurityScheme
        }
    };
    //ע��ȫ����֤�����еĽӿڶ�����ʹ����֤��
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

#endregion ���Swaggerע��

#region ���Autofac

builder.Host.UserAutoFac();

#endregion ���Autofac

builder.Services.AddDbContext<RoadOfGropingDbContext>(option =>
{
    option.UseSqlServer(config.GetConnectionString("Default"));
});

builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log4Extention.InitLog4(loggingBuilder);
});

#region ��������֤JWT

//builder.Services.AddSingleton<
//    IAuthorizationMiddlewareResultHandler, RoadOfGropingAuthorizationMiddleware>();
//��ȡjwt������
var jwtTokenConfig = builder.Configuration.GetSection("JWT").Get<JwtModel>();

//���jwt��֤��
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //��֤ģʽ
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    //��ѯģʽ
})
.AddCookie(options =>
{
    //cokkie����
    options.Cookie.Name = "BearerCokkie";
    //cokkie����ʱ��
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //cokkie���û�������ʱ��
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
        ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
        ValidAudience = jwtTokenConfig.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.SecretKey)),
        ClockSkew = TimeSpan.FromMinutes(1)  //��token����ʱ����֤������ʱ��
    };
    //���jwt���ڣ��ڷ��ص�header�м���Token-Expired�ֶ�Ϊtrue��ǰ���ڻ�ȡ����headerʱ�ж�
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
        //�˴�ΪȨ����֤ʧ�ܺ󴥷����¼�
        OnChallenge = context =>
        {
            //�˴�����Ϊ��ֹ.Net CoreĬ�ϵķ������ͺ����ݽ�����������ҪŶ������
            context.HandleResponse();

            //�Զ����Լ���Ҫ���ص����ݽ����������Ҫ���ص���Json����ͨ������Newtonsoft.Json�����ת��
            var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "�ܱ�Ǹ������Ȩ���ʸýӿ�" });
            //�Զ��巵�ص���������
            context.Response.ContentType = "application/json";
            //�Զ��巵��״̬�룬Ĭ��Ϊ401
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //���Json���ݽ��
            context.Response.WriteAsync(payload);
            return Task.CompletedTask;
        }
    };
});

#endregion ��������֤JWT

#region ͳһ����ֵ

builder.Services.AddMvc(options =>
{
    options.Filters.Add<ApiResultFilterAttribute>();
});

#endregion ͳһ����ֵ

builder.Services.AddApplication<RoadOfGropingHostModule>();

//ע��ִ�����
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

#region ���������֤

app.UseAuthentication();//����˭��ǰ
app.UseAuthorization();//�ܸ�ʲô�ں�

#endregion ���������֤

app.UseMiddleware<UnitOfWorkMiddleware>();

app.MapControllers();

app.Run();