using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RoadOfGroping.Utility.MessageCenter.Dingtalk
{
    public interface IDingtalkManager
    {
        OapiMessageCorpconversationAsyncsendV2Response SendWorkMessage(DingtalkMsg msg, List<string> userIds);

        OapiUserGetResponse GetUserInfo(string userId);

        OapiV2UserGetbymobileResponse GetByMobile(string mobile);

        OapiUserListidResponse GetUserByDept(long deptId);

        OapiV2DepartmentListsubResponse GetDepartmentList(long? deptId);

        OapiV2DepartmentListsubidResponse GetChildrenDepartmentList(long deptId);
    }

    /// <summary>
    ///     钉钉
    /// </summary>
    public class DingtalkManager : IDingtalkManager
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public DingtalkManager(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        #region 消息

        /// <summary>
        /// 发送工作通知
        /// 同一个应用相同内容的消息，同一个用户一天只能接收一次。
        /// 同一个企业内部应用在一天之内，最多可以给一个用户发送500条消息通知。
        /// 通过设置to_all_user参数全员推送消息，一天最多3次。且企业发送消息单次最多只能给5000人发送，ISV发送消息单次最多能给1000人发送。
        /// </summary>
        /// <returns></returns>
        public OapiMessageCorpconversationAsyncsendV2Response SendWorkMessage(DingtalkMsg msg, List<string> userIds)
        {
            IDingTalkClient client =
                new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
            var req = new OapiMessageCorpconversationAsyncsendV2Request();

            var config = GetDingtalkConfig();
            req.AgentId = config.AgentId;
            req.UseridList = string.Join(",", userIds.ToArray());
            msg.text.content += $"(消息发送时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss})";
            req.Msg = JsonConvert.SerializeObject(msg);
            req.SetHttpMethod("GET");
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        #endregion 消息

        /// <summary>
        ///     授权
        /// </summary>
        /// <returns></returns>
        private OapiGettokenResponse GetAccessToken(DingtalkConfig config)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/gettoken");
            var request = new OapiGettokenRequest { Appkey = config.AppKey, Appsecret = config.AppSecret };
            request.SetHttpMethod("GET");
            var response = client.Execute(request);
            if (response.Errcode == 0)
            {
                return response;
            }

            return null;
        }

        /// <summary>
        ///     获取配置
        /// </summary>
        /// <returns></returns>
        private DingtalkConfig GetDingtalkConfig()
        {
            var config = new DingtalkConfig
            {
                CorpId = _configuration["Dingtalk:CorpId"],
                ApiToken = _configuration["Dingtalk:ApiToken"],
                AgentId = Convert.ToInt64(_configuration["Dingtalk:AgentId"]),
                AppKey = _configuration["Dingtalk:AppKey"],
                AppSecret = _configuration["Dingtalk:AppSecret"]
            };
            return config;
        }

        #region 用户

        /// <summary>
        ///     获取用户信息
        /// </summary>
        /// <returns></returns>
        public OapiUserGetResponse GetUserInfo(string userId)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/get");
            var request = new OapiUserGetRequest { Userid = userId };
            request.SetHttpMethod("GET");

            var config = GetDingtalkConfig();
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var response = client.Execute(request, accessToken);
            return response;
        }

        /// <summary>
        ///     根据手机号获取userid
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public OapiV2UserGetbymobileResponse GetByMobile(string mobile)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/getbymobile");
            var req = new OapiV2UserGetbymobileRequest { Mobile = mobile };
            req.SetHttpMethod("GET");
            var config = GetDingtalkConfig();
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        /// <summary>
        ///     获取当前部门下的userid列表
        /// </summary>
        /// <returns></returns>
        public OapiUserListidResponse GetUserByDept(long deptId)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/user/listid");
            var req = new OapiUserListidRequest { DeptId = deptId };
            req.SetHttpMethod("GET");
            var config = GetDingtalkConfig();
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        #endregion 用户

        #region 部门

        /// <summary>
        ///     获取部门列表
        /// </summary>
        /// <param name="deptId">父部门ID。如果不传，默认部门为根部门，根部门ID为1。只支持查询下一级子部门，不支持查询多级子部门</param>
        /// <returns></returns>
        public OapiV2DepartmentListsubResponse GetDepartmentList(long? deptId)
        {
            IDingTalkClient client =
                new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/department/listsub");
            var req = new OapiV2DepartmentListsubRequest { DeptId = deptId };
            req.SetHttpMethod("GET");
            var config = GetDingtalkConfig();
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        /// <summary>
        ///     获取子部门ID列表
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public OapiV2DepartmentListsubidResponse GetChildrenDepartmentList(long deptId)
        {
            IDingTalkClient client =
                new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/department/listsubid");
            var req = new OapiV2DepartmentListsubidRequest { DeptId = deptId };
            req.SetHttpMethod("GET");
            var config = GetDingtalkConfig();
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        #endregion 部门

        #region 用户 - 消息中心

        public OapiMessageCorpconversationAsyncsendV2Response SendWorkMessage(DingtalkMsg msg, List<string> userIds, DingtalkConfig config)
        {
            IDingTalkClient client =
                new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
            var req = new OapiMessageCorpconversationAsyncsendV2Request();
            req.AgentId = config.AgentId;
            req.UseridList = string.Join(",", userIds.ToArray());
            msg.text.content += $"\n\r(消息发送时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss})";
            req.Msg = JsonConvert.SerializeObject(msg);
            req.SetHttpMethod("GET");
            var accessToken = GetAccessToken(config).AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        /// <summary>
        ///     获取用户信息
        /// </summary>
        /// <returns></returns>
        public OapiUserGetResponse GetUserInfoAsync(string userId, DingtalkConfig config)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/get");
            var request = new OapiUserGetRequest { Userid = userId };
            request.SetHttpMethod("GET");
            var accessToken = GetAccessToken(config)?.AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var response = client.Execute(request, accessToken);
            return response;
        }

        /// <summary>
        ///     根据手机号获取userid
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public OapiV2UserGetbymobileResponse GetByMobile(string mobile, DingtalkConfig config)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/getbymobile");
            var req = new OapiV2UserGetbymobileRequest { Mobile = mobile, SupportExclusiveAccountSearch = config.SupportExclusiveAccountSearch };
            req.SetHttpMethod("GET");
            var accessToken = GetAccessToken(config)?.AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        /// <summary>
        ///     获取当前部门下的userid列表
        /// </summary>
        /// <returns></returns>
        public OapiUserListidResponse GetUserByDept(long deptId, DingtalkConfig config)
        {
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/user/listid");
            var req = new OapiUserListidRequest { DeptId = deptId };
            req.SetHttpMethod("GET");
            var accessToken = GetAccessToken(config)?.AccessToken;
            if (accessToken == null)
            {
                return null;
            }

            var rsp = client.Execute(req, accessToken);
            return rsp;
        }

        #endregion 用户 - 消息中心
    }
}