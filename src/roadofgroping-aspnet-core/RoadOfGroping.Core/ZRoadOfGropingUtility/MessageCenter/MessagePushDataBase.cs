namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter
{
    /// <summary>
    /// 短信通知基础Dto
    /// </summary>
    public class MessagePushDataBase
    {
        /// <summary>
        /// 消息唯一标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 调用者唯一标识，标识可重复
        /// </summary>
        public Guid Caller { get; set; }

        /// <summary>
        /// 推送渠道
        /// </summary>
        public PushChannelType PushChannelType { get; set; }
    }
}