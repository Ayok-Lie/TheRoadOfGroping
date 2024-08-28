using RoadOfGroping.Common.Dependency;

namespace RoadOfGroping.Utility.ApiResult.Modules
{
    public class ResultModel : IResultModel, IDependency
    {
        public int StatusCode { get; set; }

        public string? Message { get; set; } = String.Empty;

        public object? Result { get; set; } = null;
    }
}