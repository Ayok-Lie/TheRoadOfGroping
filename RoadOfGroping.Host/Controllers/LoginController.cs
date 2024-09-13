using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Core.Users;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token;
using RoadOfGroping.Repository.UserSession;

namespace RoadOfGroping.Host.Controllers
{
    [ApiController]
    [DisabledUnitOfWork(true)]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private TokenHelper _tokenHelper;
        private ILogger<LoginController> _logger;
        private IUserManager _userManager;

        public LoginController(TokenHelper tokenHelper, ILogger<LoginController> logger, IUserManager userManager)
        {
            _tokenHelper = tokenHelper;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Login(UserInfo user)
        {
            var userinfo = await _userManager.Login(user.username, user.password);
            if (userinfo == null)
            {
                throw new Exception("账号密码错误");
            }
            // 设置Token的Claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(LoginClaimTypes.UserName, userinfo.UserName!), //HttpContext.User.Identity.Name
                new Claim(LoginClaimTypes.UserId, userinfo.Id!.ToString()),
                new Claim(
                    LoginClaimTypes.Expiration,
                    DateTimeOffset
                        .Now.AddMinutes(30)
                        .ToString()
                ),
            };
            var token = _tokenHelper.CreateToken(claims);

            Response.Cookies.Append(
                "access-token",
                token,
                new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(
                        30
                    )
                }
            );

            _logger.LogInformation("登录成功");
            return token;
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