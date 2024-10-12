namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Email
{
    public class EmailSettings
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
    }
}