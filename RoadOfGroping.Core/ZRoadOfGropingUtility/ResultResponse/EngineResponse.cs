namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse
{
    [Serializable]
    public class EngineResponse<TResult> : ResponseBase
    {
        public TResult? Result { get; set; }

        public EngineResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

        public EngineResponse(int code, bool isSuccess)
        {
            StatusCode = code;
            Success = isSuccess;
        }

        public EngineResponse()
        {
            Success = true;
        }

        public EngineResponse(ErrorInfo error, bool unAuthorizedRequest = false)
        {
            Error = error;
            UnAuthorizedRequest = unAuthorizedRequest;
            Success = false;
        }
    }

    [Serializable]
    public class EngineResponse : EngineResponse<object>
    {
        public EngineResponse(object result) : base(result)
        {
        }

        public EngineResponse(int code, bool isSuccess) : base(code, isSuccess)
        {
        }

        public EngineResponse(ErrorInfo error, bool unAuthorizedRequest) : base(error, unAuthorizedRequest)
        {
        }

        public EngineResponse(object result, bool _unAuthorizedRequest) : base(result)
        {
            UnAuthorizedRequest = _unAuthorizedRequest;
        }

        public EngineResponse() : base()
        {
        }
    }
}