using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Application.AccountService;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Core.Users.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;

namespace RoadOfGroping.Host.Controllers
{
    [ApiController]
    [DisabledUnitOfWork(true)]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private ILogger<LoginController> _logger;
        private IAccountAppService _userManager;
        private IAuthTokenService authTokenService;

        public LoginController(ILogger<LoginController> logger, IAccountAppService userManager, IAuthTokenService authTokenService)
        {
            _logger = logger;
            _userManager = userManager;
            this.authTokenService = authTokenService;
        }

        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<UserLoginDto> Login(LoginDto user)
        {
            return await _userManager.Login(user);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UserLoginDto> RefreshAuthToken(AuthTokenDto auth)
        {
            var tokenInfo = await authTokenService.RefreshAuthTokenAsync(auth);
            return new UserLoginDto()
            {
                AccessToken = tokenInfo.AccessToken,
                RefreshToken = tokenInfo.RefreshToken
            };
        }

        /// <summary>
        /// 登录功能 --只能用于Cookie校验，无法用于jwt校验
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task AbandonedLogin(LoginDto user)
        {
            var userinfo = await _userManager.Login(user);
            if (userinfo == null)
            {
                throw new Exception("账号密码错误");
            }
            // 设置Token的Claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userinfo.UserName!), //HttpContext.User.Identity.Name
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(
                    ClaimTypes.Expiration,
                    DateTimeOffset
                        .Now.AddMinutes(30)
                        .ToString()
                ),
            };

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            var principal = new ClaimsPrincipal(identity);

            var aaaa = principal.Identity;
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _logger.LogInformation("登录成功");
        }

        [HttpPost]
        public void LoginOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}

//https://blog.csdn.net/jasonsong2008/article/details/89226705