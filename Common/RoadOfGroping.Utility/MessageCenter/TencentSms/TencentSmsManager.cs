using Microsoft.Extensions.Logging;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace RoadOfGroping.Utility.MessageCenter.TencentSms
{
    /// <summary>
    /// 腾讯云短信服务接口
    /// </summary>
    public interface ITencentSmsManager
    {
        Task<SendSmsResponse> SendMessage(TencentSmsMessagePushData data);
    }

    /// <summary>
    /// 腾讯云短信服务服务
    /// </summary>
    public class TencentSmsManager : ITencentSmsManager
    {
        private readonly ILogger<TencentSmsManager> _logger;

        public TencentSmsManager(ILogger<TencentSmsManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 腾讯云短信通知服务
        /// 官方地址：https://cloud.tencent.com/document/product/382/52071
        /// </summary>
        /// <param name="data"></param>
        public async Task<SendSmsResponse> SendMessage(TencentSmsMessagePushData data)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = data.SecretId,
                    SecretKey = data.SecretKey,
                };

                ClientProfile clientProfile = new ClientProfile();
                clientProfile.SignMethod = ClientProfile.SIGN_TC3SHA256;
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Timeout = 10;
                httpProfile.Endpoint = data.Endpoint;
                SmsClient client = new SmsClient(cred, "ap-guangzhou", clientProfile);
                SendSmsRequest req = new SendSmsRequest();
                req.SmsSdkAppId = data.SmsSdkAppId;
                req.SignName = data.SignName;
                req.TemplateId = data.TemplateId;
                req.TemplateParamSet = data.TemplateParamSet;
                req.PhoneNumberSet = data.PhoneNumberSet;
                SendSmsResponse resp = await client.SendSms(req);
                return resp;
            }
            catch (Exception e)
            {
                _logger?.LogError(e.Message);
                throw;
            }
        }
    }
}