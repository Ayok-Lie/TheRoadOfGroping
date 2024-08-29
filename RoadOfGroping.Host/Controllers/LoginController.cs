using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Utility.Token;

namespace RoadOfGroping.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private TokenHelper _tokenHelper;
        private ILogger<LoginController> _logger;

        public LoginController(TokenHelper tokenHelper, ILogger<LoginController> logger)
        {
            _tokenHelper = tokenHelper;
            _logger = logger;
        }

        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(UserInfo user)
        {
            if (user.username == "admin" && user.password == "123456")
            {
                var token = _tokenHelper.CreateJwtToken();
                Response.Cookies.Append(
                    "access-token", token,
                new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(
                        30
                    )
                });
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("登录成功");
                //var claims = new[]
                //{
                //    new Claim(ClaimTypes.Name, name),
                //};
                //var userIdentity = new ClaimsIdentity(claims, "login");
                //var userPrincipal = new ClaimsPrincipal(userIdentity);
                //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                //HttpContext.Request.Headers["Authorization"] = "Bearer " + _tokenHelper.CreateJwtToken();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public void LoginOut()
        {
            var identity = User.Identity as ClaimsIdentity;
            // 获取声明中的令牌
            var tokenClaim = identity?.Claims.FirstOrDefault();
            var token = tokenClaim?.Value;
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public class UserInfo
        {
            public string username { get; set; }

            public string password { get; set; }
        }
    }
}

//https://blog.csdn.net/jasonsong2008/article/details/89226705