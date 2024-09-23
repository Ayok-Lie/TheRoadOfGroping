using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Host.UnifyResult.Fiters
{
    public class AutoFromBodyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.ContentType != null && context.HttpContext.Request.ContentType.Contains("application/json"))
            {
                var body = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
                object obj = JsonSerializer.Deserialize(body, context.ActionDescriptor.Parameters[0].ParameterType);
                context.ActionArguments[context.ActionDescriptor.Parameters[0].Name] = obj;
            }

            await next();
        }
    }
}