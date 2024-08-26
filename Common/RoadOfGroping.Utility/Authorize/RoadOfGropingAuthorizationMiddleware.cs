using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace RoadOfGroping.Utility.Authorize
{
    public class RoadOfGropingAuthorizationMiddleware : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            // 检查是否存在 Show404Requirement 的授权失败
            //if (authorizeResult.Forbidden
            //    && authorizeResult.AuthorizationFailure!.FailedRequirements
            //        .OfType<Show404Requirement>().Any())
            //{
            //    // 如果存在 Show404Requirement 的授权失败，则抛出 KeyNotFoundException 异常
            //    // 这会使资源看起来像不存在一样返回 404 错误
            //    throw new KeyNotFoundException();
            //}

            // 如果没有满足条件的授权失败，则抛出 ApplicationException
            // 表示无效的令牌
            throw new ApplicationException("Invalid token");
        }
    }

    public class Show404Requirement : IAuthorizationRequirement
    { }
}