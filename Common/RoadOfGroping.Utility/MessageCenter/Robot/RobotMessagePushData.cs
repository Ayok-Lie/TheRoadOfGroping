namespace RoadOfGroping.Utility.MessageCenter.Robot
{
    /// <summary>
    /// 机器人消息通知Dto
    /// 官方地址：https://cloud.tencent.com/document/product/1263/71892
    /// </summary>
    public class RobotMessagePushData : MessagePushDataBase
    {
        /// <summary>
        /// 群机器人webhook地址
        /// </summary>
        public string WebHookUrl { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 飞书机器人信息详情
        /// </summary>
        public FeiShuContent FeiShuContent { get; set; }
        /// <summary>
        /// 企业微信机器人信息详情
        /// </summary>
        public WechatContent WechatContent { get; set; }
        /// <summary>
        /// 钉钉机器人信息详情
        /// </summary>
        public DingdingContent DingdingContent { get; set; }
    }
    /// <summary>
    /// 飞书机器人信息详情
    /// </summary>
    public class FeiShuContent
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string msg_Type { get; set; }
        /// <summary>
        /// 消息详细内容
        /// </summary>
        public FeiShuText content { get; set; }
        /// <summary>
        /// 飞书机器人消息内容类
        /// </summary>
        public class FeiShuText
        {
            /// <summary>
            /// 消息文本内容
            /// </summary>
            public string text { get; set; }
        }
    }
    /// <summary>
    /// 企业微信机器人信息详情
    /// </summary>
    public class WechatContent
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string msg_Type { get; set; }
        /// <summary>
        /// 消息详细内容
        /// </summary>
        public WechatText text { get; set; }
        /// <summary>
        /// 企业微信机器人消息内容类
        /// </summary>
        public class WechatText
        {
            /// <summary>
            /// 消息文本内容
            /// </summary>
            public string content { get; set; }
        }
    }
    /// <summary>
    /// 钉钉机器人信息详情
    /// </summary>
    public class DingdingContent
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string msg_Type { get; set; }
        /// <summary>
        /// 消息详细内容
        /// </summary>
        public DingdingText text { get; set; }
        /// <summary>
        /// 钉钉机器人消息内容类
        /// </summary>
        public class DingdingText
        {
            /// <summary>
            /// 消息文本内容
            /// </summary>
            public string content { get; set; }
        }
    }
}
