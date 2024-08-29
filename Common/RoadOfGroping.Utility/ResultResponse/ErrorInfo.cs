namespace RoadOfGroping.Utility.ResultResponse
{
    public class ErrorInfo
    {
        public object Message { get; set; }

        public ErrorInfo(string error)
        {
            Message = error;
        }

        public ErrorInfo()
        { }
    }
}