using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Utility.ResultResponse;

namespace RoadOfGroping.Utility.ApiResult
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        private readonly IActionResultWrapFactory _actionResultWrapFactory;
        private ResultHelper _result;

        public ApiResultFilterAttribute(IActionResultWrapFactory actionResultWrapFactory, ResultHelper result)
        {
            _actionResultWrapFactory = actionResultWrapFactory;
            _result = result;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    var objectResult = context.Result as ObjectResult;
        //    if (context.ActionDescriptor.EndpointMetadata.Any(em => em is SkipActionFilterAttribute)) return;
        //    var code = objectResult != null ? objectResult.StatusCode : 200;
        //    context.Result = _result.GetResult((int)code, string.Empty, objectResult?.Value);
        //}

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var action = context.ActionDescriptor as ControllerActionDescriptor;
            if (action == null)
            {
                return;
            }

            var controllerType = context.Controller.GetType();
            if (controllerType.GetCustomAttributes(typeof(SkipActionFilterAttribute), true).Any())
            {
                return;
            }

            var methodInfo = action.MethodInfo;
            if (!methodInfo.GetCustomAttributes(typeof(SkipActionFilterAttribute), true).Any())
            {
                _actionResultWrapFactory.CreateContext(context).Wrap(context);
            }
        }
    }
}