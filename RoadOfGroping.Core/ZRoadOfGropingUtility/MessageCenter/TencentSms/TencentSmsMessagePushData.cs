namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.TencentSms
{
    /// <summary>
    /// 腾讯云短信通知Dto
    /// 官方地址：https://cloud.tencent.com/document/product/382/52071
    /// </summary>
    public class TencentSmsMessagePushData : MessagePushDataBase
    {
        /// <summary>
        /// 腾讯云 API 密钥中的 SecretId。
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// 腾讯云 API 密钥中的 SecretId。
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 短信服务 API 的域名。
        /// </summary>
        public string Endpoint => "sms.tencentcloudapi.com";

        /// <summary>
        /// 短信 SDK AppID，也就是应用 ID。
        /// </summary>
        public string SmsSdkAppId { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public string SignName { get; set; }

        /// <summary>
        /// 短信模板 ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 模板参数数组，用于替换模板中的占位符
        /// </summary>
        public string[] TemplateParamSet { get; set; }

        /// <summary>
        /// 短信接收号码数组，支持多个号码。
        /// </summary>
        public string[] PhoneNumberSet { get; set; }
    }
}