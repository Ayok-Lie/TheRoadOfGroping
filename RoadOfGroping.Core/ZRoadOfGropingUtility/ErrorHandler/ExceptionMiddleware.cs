using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio.Exceptions;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ErrorHandler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private IHostEnvironment _environment;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            this._logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            ExceptionDispatchInfo edi;
            try
            {
                var task = _next(context);
                //没有完成继续异步委托执行
                if (!task.IsCompletedSuccessfully)
                {
                    await Awaited(context, () => task);
                }
                return;
            }
            catch (Exception e)
            {
                edi = ExceptionDispatchInfo.Capture(e);
            }

            await HandleException(context, edi);
        }

        //捕获后续管道的异常
        private async Task Awaited(HttpContext context, Func<Task> func)
        {
            ExceptionDispatchInfo? edi = null;
            try
            {
                await func.Invoke();
            }
            catch (Exception exception)
            {
                edi = ExceptionDispatchInfo.Capture(exception);
            }
            if (edi != null)
            {
                await HandleException(context, edi);
            }
        }

        private async Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
        {
            if (context.Response.HasStarted)
            {
                // 响应开始抛出异常终止响应
                edi.Throw();
            }
            switch (edi.SourceException)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        break;
                    }
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case KeyNotFoundException ex:

                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }
            var requestPath = context.Request.Path;
            var exception = edi.SourceException;

            var logMsg = $"错误信息：{exception.Message}";
            _logger.LogError(logMsg);
            EngineResponse response = new EngineResponse(new ErrorInfo()
            {
                Message = _environment.IsDevelopment() ? logMsg : exception.Message
            }, false);
            response.Success = false;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(response);

            context.Response.OnCompleted(async () =>
            {
            });
        }
    }
}