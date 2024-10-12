using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations;

namespace RoadOfGroping.Host.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : RoadOfGropingControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestController> _logger;

        public TestController(IServiceProvider serviceProvider, ILogger<TestController> logger) : base(serviceProvider)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取多语言
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            //var path = Localizer["Name"].SearchedLocation;
            return L("Name");
        }

        /// <summary>
        /// 测试赋值请求头
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionAuthorize(AppPermissions.User.Delete)]
        public async Task<string> HttpClient()
        {
            using (var httpClient = new HttpClient())
            {
                // 创建HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");

                // 添加自定义请求头
                request.Headers.Add("Authorization", "49546546435464343434343");

                // 发送请求
                var response = await httpClient.SendAsync(request);

                // 输出响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                throw new KeyNotFoundException("Get User failed");
            }
        }

        [HttpGet]
        //public async Task<string> CilentGetTest()
        //{
        //    #region  模拟客户端请求api接口
        //    var client = new HttpClient();
        //    //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5100/connect/token");  //要请求的验证的地址
        //    ////模拟客户端验证
        //    //var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    //{
        //    //    Address = "https://localhost:7285/api/Login/LoginTest",    //identity自带的获取验证令牌的路由
        //    //    ClientId = "client",       //对应server端定义的配置
        //    //    ClientSecret = "secret",
        //    //    Scope = "api"
        //    //});

        //    //if (tokenResponse.IsError)
        //    //{
        //    //    return tokenResponse.Error;
        //    //}

        //    client.SetBearerToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcmVmZXJyZWRfdXNlcm5hbWUiOiJhZG1pbiIsInNjb3BlIjoiVXBkYXRlUGVybWlzc2lvbiIsImV4cCI6MTcyNjI5MzYxNiwibmJmIjoxNzI2MjkxODE2LCJpc3MiOiJjb20ueHh4LmF1dGhTZXJ2aWNlIiwiYXVkIjoib2F1dGgvY29tLnh4eC5hdXRoU2VydmljZSJ9.HhZbqXWLGY41TbQsvCKfqdXeYWCAC8ftBPMomJcZPxM");  //要将Token发送到API，通常使用HTTP Authorization标头。 这是使用SetBearerToken扩展方法完成的：

        //    var response = await client.GetAsync("https://localhost:7285/api/Test/HttpClient");  //需要请求接口/controller的地址
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return response.StatusCode.ToString();
        //    }
        //    else
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        return content;
        //    }
        //    #endregion

        //}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTestResult(string userId)
        {
            Console.WriteLine("测试一下输出日志");
            _logger.LogInformation("日志输出了");
            throw new Exception("Test exception");
            //return Ok(_user);
        }
    }
}