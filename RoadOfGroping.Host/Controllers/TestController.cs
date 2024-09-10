using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoadOfGroping.Host.Controllers
{
    /// <summary>
    /// ���Կ�����
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestController> _logger;

        /// <summary>
        /// ���Կ�����
        /// </summary>
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// ���Ը�ֵ����ͷ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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