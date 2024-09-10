using RoadOfGroping.Common.Dependency;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ApiResult.Modules
{
    public class ResultModel : IResultModel, IDependency
    {
        public int StatusCode { get; set; }

        public string? Message { get; set; } = String.Empty;

        public object? Result { get; set; } = null;
    }
}