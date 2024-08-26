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

        public LoginController(TokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(string name, string pwd)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
            };
            var userIdentity = new ClaimsIdentity(claims, "login");
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
            HttpContext.Request.Headers["Authorization"] = "Bearer " + _tokenHelper.CreateJwtToken();
            return Ok();
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
    }
}

//https://blog.csdn.net/jasonsong2008/article/details/89226705