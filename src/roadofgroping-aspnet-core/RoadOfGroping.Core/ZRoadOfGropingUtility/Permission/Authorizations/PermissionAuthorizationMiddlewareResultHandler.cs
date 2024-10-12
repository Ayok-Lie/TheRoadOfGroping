//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authorization.Policy;
//using Microsoft.AspNetCore.Http;
//using RoadOfGroping.Common.DependencyInjection;

//namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations
//{
//    public class PermissionAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler, ISingletonDependency
//    {
//        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
//        {
//            if (!authorizeResult.Succeeded || authorizeResult.Challenged)
//            {
//                var reason = authorizeResult?.AuthorizationFailure?.FailureReasons.FirstOrDefault();
//                var isLogin = context?.User?.Identity?.IsAuthenticated ?? false;
//                var path = context?.Request?.Path ?? "";
//                context!.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                await context.Response.WriteAsJsonAsync(isLogin ? reason?.Message : "请先登录系统");
//                return;
//            }
//            await next(context);
//        }
//    }

//    public class ErrorInfo
//    {
//        public object Message { get; set; }
//    }
//}