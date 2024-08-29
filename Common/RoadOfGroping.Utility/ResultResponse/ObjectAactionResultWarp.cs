using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Utility.ResultResponse;

public class ObjectAactionResultWarp : IActionResultWarp
{
    public void Wrap(FilterContext context)
    {
        ObjectResult objectResult = null;

        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                objectResult = resultExecutingContext.Result as ObjectResult;
                break;
        }

        if (objectResult == null)
        {
            throw new Exception("Action Result should be JsonResult!");
        }

        var statusCode = context.HttpContext.Response.StatusCode;

        if (!(objectResult.Value is ResponseBase))
        {
            var isSuccess = statusCode == StatusCodes.Status200OK;
            var response = new EngineResponse();
            if (isSuccess) { response.Result = objectResult.Value; } else { response.Error = new ErrorInfo { Message = objectResult.Value }; }
            response.Success = isSuccess;
            response.StatusCode = statusCode;
            //response.Extras = HttpExtension.Take();
            objectResult.Value = response;
            objectResult.DeclaredType = typeof(EngineResponse);
        }
    }
}