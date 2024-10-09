using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ApiResult
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        private readonly IActionResultWrapFactory _actionResultWrapFactory;

        public ApiResultFilterAttribute(IActionResultWrapFactory actionResultWrapFactory)
        {
            _actionResultWrapFactory = actionResultWrapFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

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