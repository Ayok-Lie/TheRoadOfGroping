using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

public interface IActionResultWarp
{
    void Wrap(FilterContext context);
}