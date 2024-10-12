using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Permission.Authorizations;

namespace RoadOfGroping.Host.Controllers
{
    /// <summary>
    /// ���Կ�����
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
        /// ��ȡ������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            //var path = Localizer["Name"].SearchedLocation;
            return L("Name");
        }

        /// <summary>
        /// ���Ը�ֵ����ͷ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionAuthorize(AppPermissions.User.Delete)]
        public async Task<string> HttpClient()
        {
            using (var httpClient = new HttpClient())
            {
                // ����HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");

                // ����Զ�������ͷ
                request.Headers.Add("Authorization", "49546546435464343434343");

                // ��������
                var response = await httpClient.SendAsync(request);

                // �����Ӧ����
                string responseBody = await response.Content.ReadAsStringAsync();

                throw new KeyNotFoundException("Get User failed");
            }
        }

        [HttpGet]
        //public async Task<string> CilentGetTest()
        //{
        //    #region  ģ��ͻ�������api�ӿ�
        //    var client = new HttpClient();
        //    //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5100/connect/token");  //Ҫ�������֤�ĵ�ַ
        //    ////ģ��ͻ�����֤
        //    //var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    //{
        //    //    Address = "https://localhost:7285/api/Login/LoginTest",    //identity�Դ��Ļ�ȡ��֤���Ƶ�·��
        //    //    ClientId = "client",       //��Ӧserver�˶��������
        //    //    ClientSecret = "secret",
        //    //    Scope = "api"
        //    //});

        //    //if (tokenResponse.IsError)
        //    //{
        //    //    return tokenResponse.Error;
        //    //}

        //    client.SetBearerToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcmVmZXJyZWRfdXNlcm5hbWUiOiJhZG1pbiIsInNjb3BlIjoiVXBkYXRlUGVybWlzc2lvbiIsImV4cCI6MTcyNjI5MzYxNiwibmJmIjoxNzI2MjkxODE2LCJpc3MiOiJjb20ueHh4LmF1dGhTZXJ2aWNlIiwiYXVkIjoib2F1dGgvY29tLnh4eC5hdXRoU2VydmljZSJ9.HhZbqXWLGY41TbQsvCKfqdXeYWCAC8ftBPMomJcZPxM");  //Ҫ��Token���͵�API��ͨ��ʹ��HTTP Authorization��ͷ�� ����ʹ��SetBearerToken��չ������ɵģ�

        //    var response = await client.GetAsync("https://localhost:7285/api/Test/HttpClient");  //��Ҫ����ӿ�/controller�ĵ�ַ
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
            Console.WriteLine("����һ�������־");
            _logger.LogInformation("��־�����");
            throw new Exception("Test exception");
            //return Ok(_user);
        }
    }
}