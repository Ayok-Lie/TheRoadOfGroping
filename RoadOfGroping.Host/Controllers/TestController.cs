using log4net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.OrderTest;
using RoadOfGroping.Core.OrderTest.Entity;

namespace RoadOfGroping.Host.Controllers
{
    /// <summary>
    /// 测试控制器
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
        /// 测试控制器
        /// </summary>
        public TestController(ILogger<TestController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// 测试赋值请求头
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
            Console.WriteLine("测试一下输出日志");
            _logger.LogInformation("日志输出了");
            throw new Exception("Test exception");
            //return Ok(_user);
        }
    }
}