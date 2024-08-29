using Microsoft.AspNetCore.Mvc.Filters;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Utility.ResultResponse;

public interface IActionResultWrapFactory : ITransientDependency
{
    IActionResultWarp CreateContext(FilterContext filterContext);
}