using System.Diagnostics;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Host.Extensions;

namespace RoadOfGroping.Host.Middlewares
{
    /// <summary>
    /// 处理 API 端点的响应头和文件附件。
    /// </summary>
    [DebuggerStepThrough]
    public class ApiInfoMiddleware : IMiddleware, ITransientDependency
    {
        /// <summary>
        /// 处理 API 端点的响应头和文件附件。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.OnStarting([DebuggerStepThrough] () =>
            {
                if (context.Response.StatusCode == StatusCodes.Status200OK
                && context.Response.Headers["Content-Type"].ToString() == "application/vnd.ms-excel")
                {
                    context.FileAttachmentHandle($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.xlsx");
                }
                if (context.Response.StatusCode == StatusCodes.Status200OK &&
                context.Response.Headers["Content-Type"].ToString() == "application/x-zip-compressed")
                {
                    context.FileAttachmentHandle($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip");
                }
                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}