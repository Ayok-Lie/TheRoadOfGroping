using Microsoft.AspNetCore.Mvc.Filters;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse;

public interface IActionResultWrapFactory : ITransientDependency
{
    IActionResultWarp CreateContext(FilterContext filterContext);
}