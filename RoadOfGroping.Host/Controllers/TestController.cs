using log4net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.OrderTest;
using RoadOfGroping.Core.OrderTest.Entity;

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

        private readonly IOrderRepository orderRepository;

        /// <summary>
        /// ���Կ�����
        /// </summary>
        public TestController(ILogger<TestController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            this.orderRepository = orderRepository;
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

            throw new KeyNotFoundException("Get User failed");
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get(CancellationToken cancellationToken)
        {
            return await orderRepository.Get();

            //return Ok("Summaries");
            //await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            //return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            await orderRepository.Create();

            return Ok();
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