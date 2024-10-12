namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Email
{
    /// <summary>
    /// 邮件通知Dto
    /// </summary>
    public class EmailMessagePushData : MessagePushDataBase
    {
        /// <summary>
        /// 接收者名称
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// 接收者邮箱地址
        /// </summary>
        public string ToEmailAddress { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}