using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token
{
    public class AppJwtBearerEvents : JwtBearerEvents
    {
        public override Task MessageReceived(MessageReceivedContext context)
        {
            if (context.Request.Cookies.TryGetValue(HeaderNames.Authorization, out var token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 质询
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            // 添加标记，使前端知晓access token过期，可以使用refresh token了
            if (context.AuthenticateFailure is SecurityTokenExpiredException)
            {
                context.Response.Headers.Add("x-access-token", "expired");
            }

            return Task.CompletedTask;

            //context.HandleResponse();
            //var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "很抱歉，您无权访问该接口" });
            //context.Response.ContentType = "application/json";
            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //context.Response.WriteAsync(payload);
            //return Task.CompletedTask;
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