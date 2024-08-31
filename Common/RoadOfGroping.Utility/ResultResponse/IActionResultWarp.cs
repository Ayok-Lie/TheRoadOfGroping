using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Utility.ResultResponse;

public interface IActionResultWarp
{
    void Wrap(FilterContext context);
}