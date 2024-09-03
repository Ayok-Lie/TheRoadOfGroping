// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace RoadOfGroping.Utility.MessageCenter.AisMms
{
    /// <summary>
    /// 阿里云短信通知Dto
    /// 官方地址：https://help.aliyun.com/document_detail/419273.htm?spm=a2c4g.419298.0.0.25116cf0UnkFlr
    /// </summary>
    public class AliSmsMessagePushData : MessagePushDataBase
    {
        /// <summary>
        /// 访问密钥ID
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// 访问密钥Secret
        /// </summary>
        public string AccessKeySecret { get; set; }
        /// <summary>
        /// API网关地址
        /// </summary>
        public string Endpoint => "dysmsapi.aliyuncs.com";
        /// <summary>
        ///接收短信的手机号码，半角逗号（,）分隔。上限为1000个
        /// </summary>
        public string PhoneNumbers { get; set; }
        /// <summary>
        /// 接收短信的手机号码
        /// </summary>
        public string SignName { get; set; }
        /// <summary>
        /// 短信模板CODE
        /// </summary>
        public string TemplateCode { get; set; }
        /// <summary>
        ///短信模板变量对应的实际值，json格式 code:xxx，+模板上限500
        /// </summary>
        public string TemplateParam { get; set; }
    }
}
