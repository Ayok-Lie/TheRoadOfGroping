using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ResultExecutingContext = Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

public class JsonActionResultWrap : IActionResultWarp
{
    public void Wrap(FilterContext context)
    {
        JsonResult jsonResult = null;

        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                jsonResult = resultExecutingContext.Result as JsonResult;
                break;
        }

        if (jsonResult == null)
        {
            throw new Exception("Action Result should be JsonResult!");
        }

        var statusCode = context.HttpContext.Response.StatusCode;

        if (!(jsonResult.Value is ResponseBase))
        {
            var isSuccess = statusCode == StatusCodes.Status200OK;
            var response = new EngineResponse();
            if (isSuccess) { response.Result = jsonResult.Value; } else { response.Error = new ErrorInfo { Message = jsonResult.Value }; }
            response.Success = isSuccess;
            response.StatusCode = statusCode;
            //response.Extras = HttpExtension.Take();
            jsonResult.Value = response;
        }
    }
}