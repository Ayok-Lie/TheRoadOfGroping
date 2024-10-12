using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Host.UnifyResult.Fiters
{
    public class EnsureJsonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                // 强制将ContentType设置为 application/json
                if (request.ContentType != null && !request.ContentType.Contains("application/json"))
                {
                    context.Result = new BadRequestObjectResult("Content type 'application/json' is required for this endpoint.");
                }
            }

            base.OnActionExecuting(context);
        }
    }
}