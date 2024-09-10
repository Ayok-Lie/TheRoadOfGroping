using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

public class ActionEmptyResultWrap : IActionResultWarp
{
    public void Wrap(FilterContext context)
    {
        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                var statusCode = context.HttpContext.Response.StatusCode;
                var isSuccess = statusCode == StatusCodes.Status200OK;
                resultExecutingContext.Result = new ObjectResult(new EngineResponse(statusCode, isSuccess));
                return;
        }
    }
}