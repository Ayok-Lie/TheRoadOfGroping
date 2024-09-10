namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Email
{
    /// <summary>
    /// 邮件通知Dto
    /// </summary>
    public class EmailMessagePushData : MessagePushDataBase
    {
        /// <summary>
        /// 邮箱服务器
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 发送者名称
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

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