using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Utility.ApiResult
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        private ResultHelper _result;

        public ApiResultFilterAttribute(ResultHelper result)
        {
            _result = result;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var objectResult = context.Result as ObjectResult;
            var code = objectResult != null ? objectResult.StatusCode : 200;
            context.Result = _result.GetResult((int)code, string.Empty, objectResult?.Value);
        }
    }
}

//最后使用badRequest()发现会出现报错，是在ApiResultFilterAttribute中OnResultExecuting的objectResult为null，
//    所以context.Result = _result.GetResult((int)code, string.Empty, objectResult.Value); 会出现错误