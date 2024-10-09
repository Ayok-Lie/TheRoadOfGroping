using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

public class NullAactionResultWrap : IActionResultWarp
{
    public void Wrap(FilterContext context)
    {
        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                resultExecutingContext.Result = new ObjectResult(new EngineResponse());
                return;
        }
    }
}