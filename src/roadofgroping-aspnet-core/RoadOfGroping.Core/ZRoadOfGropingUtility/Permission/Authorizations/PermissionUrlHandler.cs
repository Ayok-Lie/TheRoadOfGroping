using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using static RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations.PermissionUrlRequirement;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
{
    public class PermissionUrlHandler : AuthorizationHandler<PermissionUrlRequirement>
    { /// <summary>
      /// 验证方案提供对象 /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        public PermissionUrlHandler(IAuthenticationSchemeProvider schemes)
        {
            Schemes = schemes;
        } // 重载异步处理程序

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionUrlRequirement requirement)
        {
            //数据库读取数据
            var urlData = new List<PermissionUrl>();

            requirement.Permissions = urlData; //从AuthorizationHandlerContext转成HttpContext，以便取出表头信息
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext).HttpContext; //请求Url
            var questUrl = httpContext.Request.Path.Value.ToLower(); //判断请求是否停止
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>(); foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler; if (handler != null && await handler.HandleRequestAsync())
                {
                    context.Fail(); return;
                }
            } //判断请求是否拥有凭据，即有没有登录
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync(); if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name); //result?.Principal不为空即登录成功
                if (result?.Principal != null)
                {
                    httpContext.User = result.Principal; //权限中是否存在请求的url
                    if (requirement.Permissions.GroupBy(g => g.Url).Where(w => w.Key?.ToLower() == questUrl).Count() > 0)
                    { // 获取当前用户的角色信息
                        var currentUserRoles = (from item in httpContext.User.Claims where item.Type == requirement.ClaimType select item.Value).ToList(); //验证权限
                        if (currentUserRoles.Count <= 0 || requirement.Permissions.Where(w => currentUserRoles.Contains(w.Role) && w.Url.ToLower() == questUrl).Count() <= 0)
                        {
                            context.Fail(); return; // 可以在这里设置跳转页面，不过还是会访问当前接口地址的
                            httpContext.Response.Redirect(requirement.DeniedAction);
                        }
                    }
                    else
                    {
                        context.Fail(); return;
                    } //判断过期时间
                    if ((httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null && DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail(); return;
                    }
                    return;
                }
            } //判断没有登录时，是否访问登录的url,并且是Post请求，并且是form表单提交类型，否则为失败
            if (!questUrl.Equals(requirement.LoginPath.ToLower(), StringComparison.Ordinal) && (!httpContext.Request.Method.Equals("POST") || !httpContext.Request.HasFormContentType))
            {
                context.Fail(); return;
            }
            context.Succeed(requirement);
        }
    }

    /// <summary>
    /// 必要参数类， /// 继承 IAuthorizationRequirement，用于设计自定义权限处理器PermissionHandler /// 因为AuthorizationHandler 中的泛型参数 TRequirement 必须继承 IAuthorizationRequirement /// </summary>
    public class PermissionUrlRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 用户权限集合 /// </summary>
        public List<PermissionUrl> Permissions { get; set; } /// <summary>

                                                             /// 无权限action /// </summary>
        public string DeniedAction { get; set; } /// <summary>

                                                 /// 认证授权类型 /// </summary>
        public string ClaimType { internal get; set; } /// <summary>

                                                       /// 请求路径 /// </summary>
        public string LoginPath { get; set; } = "/Api/Login"; /// <summary>

                                                              /// 发行人 /// </summary>
        public string Issuer { get; set; } /// <summary>

                                           /// 订阅人 /// </summary>
        public string Audience { get; set; } /// <summary>

                                             /// 过期时间 /// </summary>
        public TimeSpan Expiration { get; set; } /// <summary>

                                                 /// 签名验证 /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        public PermissionUrlRequirement(string deniedAction, List<PermissionUrl> permissions, string claimType, string issuer, string audience, SigningCredentials signingCredentials, TimeSpan expiration)
        {
            ClaimType = claimType;
            DeniedAction = deniedAction;
            Permissions = permissions;
            Issuer = issuer;
            Audience = audience;
            Expiration = expiration;
            SigningCredentials = signingCredentials;
        }

        public class PermissionUrl
        {
            /// <summary>
            /// 用户或角色或其他凭据名称 /// </summary>
            public virtual string Role { get; set; } /// <summary>

                                                     /// 请求Url /// </summary>
            public virtual string Url { get; set; }
        }
    }
}

//https://blog.csdn.net/baidu_35726140/article/details/85346532
//https://blog.csdn.net/weixin_44877917/article/details/140609294?spm=1001.2101.3001.6650.2&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EYuanLiJiHua%7EPosition-2-140609294-blog-135392012.235%5Ev43%5Econtrol&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EYuanLiJiHua%7EPosition-2-140609294-blog-135392012.235%5Ev43%5Econtrol&utm_relevant_index=5