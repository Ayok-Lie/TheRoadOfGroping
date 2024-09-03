using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RoadOfGroping.Utility.MessageCenter.EnterpriseWeChat
{
    public interface IEnterpriseWeChatManager
    {
        AccessTokenResult GetAccessToken();

        void SendWorkMessage(string msg, List<string> userIds);

        GetMobileUserDto GetByMobile(string mobile);
    }

    public class EnterpriseWeChatManager : IEnterpriseWeChatManager
    {
        private readonly IConfiguration _configuration;

        private static EnterpriseWeChatConfig weixinSetting;

        private readonly ICacheTool cacheTool;

        public EnterpriseWeChatManager(IWebHostEnvironment env, ICacheTool cacheTool, IConfiguration configuration)
        {
            weixinSetting = GetDingtalkConfig();
            this.cacheTool = cacheTool;
            _configuration = configuration;
        }

        /// <summary>
        /// 请求accessToken
        /// </summary>
        /// <returns></returns>
        public AccessTokenResult GetAccessToken()
        {
            var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/gettoken");

            RestRequest request = new RestRequest();
            request.Method = Method.Get;
            //将corpid 和corpsecret 加入请求token的参数中
            request.AddQueryParameter("corpid", weixinSetting.WeixinCorpId);

            request.AddQueryParameter("corpsecret", weixinSetting.WeixinCorpSecret);

            var response = client.Execute(request); // 执行请求

            string res_text = response.Content; // 文本结果

            var tokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(res_text);

            return tokenResult;
        }

        /// <summary>
        /// 发送工作通知
        /// 同一个应用相同内容的消息，同一个用户一天只能接收一次。
        /// 同一个企业内部应用在一天之内，最多可以给一个用户发送500条消息通知。
        /// 通过设置to_all_user参数全员推送消息，一天最多3次。且企业发送消息单次最多只能给5000人发送，ISV发送消息单次最多能给1000人发送。
        /// </summary>
        /// <returns></returns>
        public void SendWorkMessage(string msg, List<string> userIds)
        {
            string users = string.Join("|", userIds);

            var tokenResult = GetStoreToken();

            msg += $"(消息发送时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss})";

            var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/message/send");
            var reqjson = new SendToUserDto()
            {
                touser = users,
                msgtype = "text",
                agentid = weixinSetting.WeixinCorpAgentId,
                safe = 0,
                text = new SendToUserContent()
                {
                    content = msg
                }
            };
            RestRequest request = new RestRequest();
            request.Method = Method.Post;
            // 序列化JSON数据
            string post_data = JsonConvert.SerializeObject(reqjson);
            // 将JSON参数添加至请求中
            request.AddQueryParameter("access_token", tokenResult.access_token);

            request.AddParameter("application/json", post_data, ParameterType.RequestBody);

            client.Execute(request); // 执行请求
        }

        /// <summary>
        ///     根据手机号获取userid
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public GetMobileUserDto GetByMobile(string mobile)
        {
            var redisKey = $"id:{mobile}";

            var cacheToken = cacheTool.Get(redisKey);
            if (cacheToken == null)
            {
                var accesUser = GetStoreUserId(mobile);
                if (accesUser.Errmsg == "ok")
                {
                    cacheTool.Set(redisKey, accesUser, TimeSpan.FromSeconds(10000));
                }
                return accesUser;
            }

            var jsonData = JsonConvert.SerializeObject(cacheToken);

            return JsonConvert.DeserializeObject<GetMobileUserDto>(jsonData);
        }

        /// <summary>
        ///     获取配置
        /// </summary>
        /// <returns></returns>
        private EnterpriseWeChatConfig GetDingtalkConfig()
        {
            var config = new EnterpriseWeChatConfig
            {
                WeixinCorpId = _configuration["EnterpriseWeChat:WeixinCorpId"],
                WeixinCorpAgentId = Convert.ToInt64(_configuration["EnterpriseWeChat:WeixinCorpAgentId"]),
                WeixinCorpSecret = _configuration["EnterpriseWeChat:WeixinCorpSecret"],
            };
            return config;
        }

        /// <summary>
        /// 缓存Token
        /// </summary>
        /// <returns></returns>
        private AccessTokenResult GetStoreToken()
        {
            var redisKey = $"id:{weixinSetting.WeixinCorpId}sec:{weixinSetting.WeixinCorpSecret}";

            var cacheToken = cacheTool.Get(redisKey);
            if (cacheToken == null)
            {
                var accesToken = GetAccessToken();
                cacheTool.Set(redisKey, accesToken, TimeSpan.FromSeconds(accesToken.expires_in - 200));
                return accesToken;
            }

            var jsonData = JsonConvert.SerializeObject(cacheToken);

            return JsonConvert.DeserializeObject<AccessTokenResult>(jsonData);
        }

        /// <summary>
        /// 缓存userid
        /// </summary>
        /// <returns></returns>
        private GetMobileUserDto GetStoreUserId(string mobile)
        {
            var tokenResult = GetStoreToken();
            if (tokenResult.access_token == null)
            {
                return null;
            }
            var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/user/getuserid");
            JObject reqjson = new JObject();
            reqjson.Add("mobile", mobile);
            RestRequest request = new RestRequest();
            request.Method = Method.Post;
            // 序列化JSON数据
            string post_data = JsonConvert.SerializeObject(reqjson);
            // 将JSON参数添加至请求中
            request.AddQueryParameter("access_token", tokenResult.access_token);

            request.AddParameter("application/json", post_data, ParameterType.RequestBody);

            var response = client.Execute(request); // 执行请求
            string res_text = response.Content; // 文本结果
            var mobileUser = JsonConvert.DeserializeObject<GetMobileUserDto>(res_text);

            return mobileUser;
        }

        //#region 异步

        ///// <summary>
        ///// 请求accessToken
        ///// </summary>
        ///// <returns></returns>
        //public async Task<AccessTokenResult> GetAccessTokenAsync(EnterpriseWeChatConfig config)
        //{
        //    var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/gettoken");

        //    RestRequest request = new RestRequest(Method.GET);
        //    //将corpid 和corpsecret 加入请求token的参数中
        //    request.AddQueryParameter("corpid", config.WeixinCorpId);

        //    request.AddQueryParameter("corpsecret", config.WeixinCorpSecret);

        //    var response = await client.ExecuteAsync(request); // 执行请求

        //    string res_text = response.Content; // 文本结果

        //    var tokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(res_text);

        //    return tokenResult;
        //}

        ///// <summary>
        ///// 异步发送通知
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="userIds"></param>
        ///// <returns></returns>
        //public async Task SendWorkMessageAsync(string msg, List<string> userIds, EnterpriseWeChatConfig config)
        //{
        //    string users = string.Join("|", userIds);

        //    var tokenResult = await GetStoreTokenAsync(config);

        //    msg += $"(消息发送时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss})";

        //    var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/message/send");
        //    var reqjson = new SendToUserDto()
        //    {
        //        touser = users,
        //        msgtype = "text",
        //        agentid = weixinSetting.WeixinCorpAgentId,
        //        safe = 0,
        //        text = new SendToUserContent()
        //        {
        //            content = msg
        //        }
        //    };
        //    RestRequest request = new RestRequest(Method.POST);
        //    // 序列化JSON数据
        //    string post_data = JsonConvert.SerializeObject(reqjson);
        //    // 将JSON参数添加至请求中
        //    request.AddQueryParameter("access_token", tokenResult.access_token);

        //    request.AddParameter("application/json", post_data, ParameterType.RequestBody);

        //    await client.ExecuteAsync(request); // 执行请求
        //}

        ///// <summary>
        /////     根据手机号获取userid
        ///// </summary>
        ///// <param name="mobile"></param>
        ///// <returns></returns>
        //public async Task<GetMobileUserDto> GetByMobileAsync(string mobile, EnterpriseWeChatConfig config)
        //{
        //    var redisKey = $"id:{mobile}";

        //    var cacheToken = await _cacheManager.GetCache(redisKey).GetOrDefaultAsync(redisKey);
        //    if (cacheToken == null)
        //    {
        //        var accesUser = await GetStoreUserIdAsync(mobile, config);
        //        if (accesUser.Errmsg == "ok")
        //        {
        //            await _cacheManager.GetCache(redisKey).SetAsync(redisKey, accesUser, TimeSpan.FromSeconds(10000));
        //        }
        //        return accesUser;
        //    }

        //    var jsonData = JsonConvert.SerializeObject(cacheToken);

        //    return JsonConvert.DeserializeObject<GetMobileUserDto>(jsonData);
        //}

        ///// <summary>
        ///// 缓存Token
        ///// </summary>
        ///// <returns></returns>
        //private async Task<AccessTokenResult> GetStoreTokenAsync(EnterpriseWeChatConfig config)
        //{
        //    var redisKey = $"id:{config.WeixinCorpId}sec:{config.WeixinCorpSecret}";

        //    var cacheToken = await _cacheManager.GetCache(redisKey).GetOrDefaultAsync(redisKey);
        //    if (cacheToken == null)
        //    {
        //        var accesToken = await GetAccessTokenAsync(config);
        //        await _cacheManager.GetCache(redisKey).SetAsync(redisKey, accesToken, TimeSpan.FromSeconds(accesToken.expires_in - 200));
        //        return accesToken;
        //    }

        //    var jsonData = JsonConvert.SerializeObject(cacheToken);

        //    return JsonConvert.DeserializeObject<AccessTokenResult>(jsonData);
        //}

        ///// <summary>
        ///// 缓存userid
        ///// </summary>
        ///// <returns></returns>
        //private async Task<GetMobileUserDto> GetStoreTokenAsync(string mobile, EnterpriseWeChatConfig config)
        //{
        //    var tokenResult = await GetStoreTokenAsync(config);
        //    if (tokenResult.access_token == null)
        //    {
        //        return null;
        //    }
        //    var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/user/getuserid");
        //    JObject reqjson = new JObject();
        //    reqjson.Add("mobile", mobile);
        //    RestRequest request = new RestRequest(Method.POST);
        //    // 序列化JSON数据
        //    string post_data = JsonConvert.SerializeObject(reqjson);
        //    // 将JSON参数添加至请求中
        //    request.AddQueryParameter("access_token", tokenResult.access_token);

        //    request.AddParameter("application/json", post_data, ParameterType.RequestBody);

        //    var response = await client.ExecuteAsync(request); // 执行请求
        //    string res_text = response.Content; // 文本结果
        //    var mobileUser = JsonConvert.DeserializeObject<GetMobileUserDto>(res_text);

        //    return mobileUser;
        //}

        ///// <summary>
        ///// 缓存userid
        ///// </summary>
        ///// <returns></returns>
        //private async Task<GetMobileUserDto> GetStoreUserIdAsync(string mobile, EnterpriseWeChatConfig config)
        //{
        //    var tokenResult = await GetStoreTokenAsync(config);
        //    if (tokenResult.access_token == null)
        //    {
        //        return null;
        //    }
        //    var client = new RestClient("https://qyapi.weixin.qq.com/cgi-bin/user/getuserid");
        //    JObject reqjson = new JObject();
        //    reqjson.Add("mobile", mobile);
        //    RestRequest request = new RestRequest(Method.POST);
        //    // 序列化JSON数据
        //    string post_data = JsonConvert.SerializeObject(reqjson);
        //    // 将JSON参数添加至请求中
        //    request.AddQueryParameter("access_token", tokenResult.access_token);

        //    request.AddParameter("application/json", post_data, ParameterType.RequestBody);

        //    var response = await client.ExecuteAsync(request); // 执行请求
        //    string res_text = response.Content; // 文本结果
        //    var mobileUser = JsonConvert.DeserializeObject<GetMobileUserDto>(res_text);

        //    return mobileUser;
        //}
        //#endregion
    }
}