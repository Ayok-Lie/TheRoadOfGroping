namespace RoadOfGroping.Utility.MessageCenter.SignalR
{
    public interface IChatService
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ReceiveMessage(object context);
    }
}