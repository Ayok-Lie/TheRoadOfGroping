using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Host.UnifyResult.Providers
{
    /// <summary>
    /// RESTful 风格返回值
    /// </summary>
    public class RESTfulResultProvider : IRESTfulResultProvider
    {
        /// <summary>
        /// 设置响应状态码
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="unifyResultSettings"></param>
        public static void SetResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
        {
            if (unifyResultSettings == null) return;

            // 篡改响应状态码
            if (unifyResultSettings.AdaptStatusCodes != null && unifyResultSettings.AdaptStatusCodes.Length > 0)
            {
                var adaptStatusCode = unifyResultSettings.AdaptStatusCodes.FirstOrDefault(u => u[0] == statusCode);
                if (adaptStatusCode != null && adaptStatusCode.Length > 0 && adaptStatusCode[0] > 0)
                {
                    context.Response.StatusCode = adaptStatusCode[1];
                    return;
                }
            }

            // 如果为 null，则所有请求错误的状态码设置为 200
            if (unifyResultSettings.Return200StatusCodes == null) context.Response.StatusCode = 200;
            // 否则只有里面的才设置为 200
            else if (unifyResultSettings.Return200StatusCodes.Contains(statusCode)) context.Response.StatusCode = 200;
            else { }
        }

        /// <summary>
        /// 异常返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
        {
            return new JsonResult(RESTfulResult(metadata.StatusCode, data: metadata.Data, errors: metadata.Errors));
        }

        /// <summary>
        /// 成功返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult OnSucceeded(ActionExecutedContext context, object data)
        {
            return new JsonResult(RESTfulResult(StatusCodes.Status200OK, true, data));
        }

        /// <summary>
        /// 验证失败/业务异常返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
        {
            return new JsonResult(RESTfulResult(metadata.StatusCode ?? StatusCodes.Status400BadRequest, data: metadata.Data, errors: metadata.ValidationResult));
        }

        /// <summary>
        /// 特定状态码返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="unifyResultSettings"></param>
        /// <returns></returns>
        public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
        {
            // 设置响应状态码
            SetResponseStatusCodes(context, statusCode, unifyResultSettings);

            switch (statusCode)
            {
                // 处理 401 状态码
                case StatusCodes.Status401Unauthorized:
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, errors: "401 Unauthorized"));
                    break;
                // 处理 403 状态码
                case StatusCodes.Status403Forbidden:
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, errors: "403 Forbidden"));
                    break;

                default: break;
            }
        }

        /// <summary>
        /// 返回 RESTful 风格结果集
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="succeeded"></param>
        /// <param name="data"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static RESTfulResult<object> RESTfulResult(int statusCode, bool succeeded = default, object data = default, object errors = default)
        {
            return new RESTfulResult<object>
            {
                StatusCode = statusCode,
                Succeeded = succeeded,
                Data = data,
                Errors = errors,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
    }
}