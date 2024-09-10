namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ApiResult.Modules
{
    public interface IResultModel
    {
        int StatusCode { get; set; }

        string? Message { get; set; }

        object? Result { get; set; }
    }
}