using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Core.Users;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;
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
        private IAuthTokenService authTokenService;

        public LoginController(TokenHelper tokenHelper, ILogger<LoginController> logger, IUserManager userManager, IAuthTokenService authTokenService)
        {
            _tokenHelper = tokenHelper;
            _logger = logger;
            _userManager = userManager;
            this.authTokenService = authTokenService;
        }

        [HttpGet]
        public string LoginTest()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "UpdatePermission"),
                new Claim(
                    ClaimTypes.Expiration,
                    DateTimeOffset
                        .Now.AddMinutes(30)
                        .ToString()
                ),
            };

            return _tokenHelper.CreateToken(claims);
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
            var auth = new UserAuthDto()
            {
                Id = userinfo.Id,
                UserName = userinfo.UserName,
                Roles = userinfo.Role
            };

            var token = await authTokenService.CreateAuthTokenAsync(auth);
            _logger.LogInformation("登录成功");
            return token.AccessToken;
        }
        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> AbandonedLogin(UserInfo user)
        {
            var userinfo = await _userManager.Login(user.username, user.password);
            if (userinfo == null)
            {
                throw new Exception("账号密码错误");
            }
            // 设置Token的Claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userinfo.UserName!), //HttpContext.User.Identity.Name
                new Claim(ClaimTypes.Role, "UpdatePermission"),
                new Claim(
                    ClaimTypes.Expiration,
                    DateTimeOffset
                        .Now.AddMinutes(30)
                        .ToString()
                ),
            };
            var token = _tokenHelper.CreateToken(claims);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _logger.LogInformation("登录成功");
            return token;
        }

        [HttpGet]
        public void LoginOut()
        {
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