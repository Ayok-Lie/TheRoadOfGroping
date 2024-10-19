using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Common.Options;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token
{
    public class AppJwtBearerEvents : JwtBearerEvents
    {
        private readonly IAuthTokenManager authTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtOptions _jwtOptions;

        public AppJwtBearerEvents(IAuthTokenManager authTokenService, IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<JwtOptions> jwtOptions)
        {
            this.authTokenService = authTokenService;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions.Value;
        }

        public override Task MessageReceived(MessageReceivedContext context)
        {
            if (context.Request.Cookies.TryGetValue(HeaderNames.Authorization, out var token))
            {
                context.Token = JsonConvert.DeserializeObject<AuthTokenDto>(token)?.AccessToken;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 质询
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Challenge(JwtBearerChallengeContext context)
        {
            // 添加标记，使前端知晓access token过期，可以使用refresh token了
            if (context.AuthenticateFailure is SecurityTokenExpiredException)
            {
                //if (!(context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "refresh_token_id")?.Value).IsNullOrEmpty())
                //{
                //    context.Response.Headers.Add("x-access-token", "expired");
                //}
                //else
                //{
                //    var accessToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //    var auth = new AuthTokenDto
                //    {
                //        AccessToken = accessToken,
                //        RefreshToken = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "refresh_token_id")?.Value,
                //    };
                //    var token = await authTokenService.RefreshAuthTokenAsync(auth);
                //    // 将访问令牌放入Cookie中
                //    _httpContextAccessor.HttpContext.Response.Cookies.Append(HeaderNames.Authorization, token.AccessToken, new CookieOptions
                //    {
                //        HttpOnly = true,
                //        IsEssential = true,
                //        MaxAge = TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays),
                //        Path = "/",
                //        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax
                //    });
                //    // 返回新的 Token 信息给前端
                //    context.Response.Headers.Add("x-access-token", token.AccessToken);
                //    context.HandleResponse();
                //    // 设置状态码和内容类型
                //    context.Response.ContentType = "application/json";
                //    context.Response.StatusCode = StatusCodes.Status200OK;
                //}
            }
            else
            {
                context.HandleResponse();
                var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "很抱歉，您无权访问该接口" });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(payload);
            }

            await Task.CompletedTask;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    }
}