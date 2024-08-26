using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Utility.Token;

namespace RoadOfGroping.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TokenController : ControllerBase
    {
        private ILogger<TokenController> _logger;
        private readonly IServiceProvider _provider;
        private TokenHelper _tokenHelper;

        public TokenController(
            ILogger<TokenController> logger,
            IServiceProvider provider,
            TokenHelper tokenHelper)
        {
            _logger = logger;
            _provider = provider;
            _tokenHelper = tokenHelper;
        }

        [AllowAnonymous]
        [HttpGet("GetTest")]
        public async Task<IActionResult> GetTestResult(string userId)
        {
            Console.WriteLine("测试一下输出日志");
            _logger.LogInformation("日志输出了");
            //throw new Exception("Test exception");
            //return Ok(_user);
            //return BadRequest();
            return Ok(_tokenHelper.CreateJwtToken());
        }
    }
}