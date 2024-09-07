namespace RoadOfGroping.Utility.MessageCenter.IHuaWeiSms
{
    /// <summary>
    /// 华为云短信通知Dto
    /// 官方地址：https://support.huaweicloud.com/api-msgsms/sms_05_0001.html
    /// </summary>
    public class HuaweiSmsMessagePushData : MessagePushDataBase
    {
        /// <summary>
        /// API请求地址
        /// </summary>
        public string ApiAddress { get; set; }

        /// <summary>
        /// 华为云应用的APP KEY。
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 华为云应用的APP SECRET。
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 国内短信签名通道号或国际/港澳台短信通道号
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        ///短信模板ID。
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 短信接收人号码，用英文逗号分隔
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 模板变量   字符串类型  array
        /// </summary>
        public string TemplateParas { get; set; }
    }
}