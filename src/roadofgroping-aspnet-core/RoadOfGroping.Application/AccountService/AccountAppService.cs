using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadOfGroping.Application.AccountService.Dtos;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Core.Users.DomainService;
using RoadOfGroping.Core.Users.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Email;
using RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;
using RoadOfGroping.Repository.DomainService;
using Yitter.IdGenerator;

namespace RoadOfGroping.Application.AccountService
{
    /// <summary>
    /// 登录、注册、密码重置、修改密码、发送邮箱验证码等功能
    /// </summary>
    public class AccountAppService : ApplicationService, IAccountAppService
    {
        private ILogger<AccountAppService> _logger;
        private IUserManager _userManager;
        private IAuthTokenManager authTokenService;
        private readonly CacheManager _cacheManager;
        private readonly IIdGenerator _idGenerator;
        private readonly IUserRolesManager _userRolesManager;
        private readonly IEmailPushManager emailPushManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountAppService(IServiceProvider serviceProvider,
            ILogger<AccountAppService> logger,
            IUserManager userManager,
            IAuthTokenManager authTokenService,
            CacheManager cacheManager,
            IIdGenerator idGenerator,
            IUserRolesManager userRolesManager,
            IEmailPushManager emailPushManager,
            IHttpContextAccessor httpContextAccessor) : base(serviceProvider)
        {
            _logger = logger;
            _userManager = userManager;
            this.authTokenService = authTokenService;
            _cacheManager = cacheManager;
            _idGenerator = idGenerator;
            _userRolesManager = userRolesManager;
            this.emailPushManager = emailPushManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UserLoginDto> Login(LoginDto dto)
        {
            var user = await _userManager.QueryAsNoTracking.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
            if (user == null)
            {
                throw new ArgumentException(L($"UserDoesNotExist", user.UserName));
            }
            if (user == null || user.Password != MD5Encryption.Encrypt($"{_idGenerator.Encode(user.Id)}{dto.Password}"))
            {
                throw new ArgumentException(L("PasswordError"));
            }

            var tokenInfo = await authTokenService.CreateAuthTokenAsync(user.Id);
            _logger.LogInformation("登录成功");
            return new UserLoginDto()
            {
                TokenInfoOutput = new TokenInfoOutput()
                {
                    AccessToken = tokenInfo.AccessToken,
                    RefreshToken = tokenInfo.RefreshToken,
                    Expires = tokenInfo.Expiration,
                },
                UserInfoOutPut = new UserInfoOutPut()
                {
                    Avater = user.Avater,
                    UserName = user.UserName,
                    NickName = user.NickName
                }
            };
        }

        /// <summary>
        /// Swagger登录功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task SwaggerLogin(LoginDto dto)
        {
            var user = await _userManager.QueryAsNoTracking.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
            if (user == null)
            {
                throw new ArgumentException(L($"UserDoesNotExist", user.UserName));
            }
            if (user == null || user.Password != MD5Encryption.Encrypt($"{_idGenerator.Encode(user.Id)}{dto.Password}"))
            {
                throw new ArgumentException(L("PasswordError"));
            }
            await authTokenService.CreateJwtTokenForSwaggerAuth(user.Id);
            _logger.LogInformation("Swagger登录成功");
        }

        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task PasswordReset(PasswordResetInput input)
        {
            var user = await _userManager.QueryAsNoTracking.FirstOrDefaultAsync(x => x.UserEmail == input.EmailAddress && x.UserName == input.UserName);
            if (user == null)
            {
                throw new ArgumentException(L($"UserDoesNotExist", user.UserName));
            }
            var newPassword = MD5Encryption.Encrypt($"{_idGenerator.Encode(user.Id)}bb123456");
            user.Password = newPassword;
            await _userManager.UpdateAsync(user);
            var en = new EmailMessagePushData();
            en.Subject = "密码重置";
            en.Content = "您的密码已重置为：bb123456，请尽快修改密码。";
            en.ToEmailAddress = input.EmailAddress;
            en.PushChannelType = Core.ZRoadOfGropingUtility.MessageCenter.PushChannelType.Email;
            en.ToName = input.UserName;
            en.Caller = Guid.NewGuid();
            await emailPushManager.SendMessage(en);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ChangePassword(ChangePasswordInput input)
        {
            if (UserSession.UserId == null)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync(); // 退出登录
                throw new ArgumentException(L("UserNotLogin"));
            }
            if (input.NewPassword != input.ConfirmPassword)
            {
                throw new ArgumentException(L("PasswordNotMatch"));
            }
            var user = await _userManager.QueryAsNoTracking.FirstOrDefaultAsync(x => x.Id == UserSession.UserId);
            if (user == null)
            {
                throw new ArgumentException(L($"UserDoesNotExist", user.UserName));
            }

            user.Password = MD5Encryption.Encrypt($"{_idGenerator.Encode(user.Id)}{input.NewPassword}");

            await _userManager.UpdateAsync(user);
            _logger.LogInformation("密码修改成功");
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task SendEmailAddressConfirmCode(SendPasswordResetCodeInput input)
        {
            var emailCache = _cacheManager.Get(input.EmailAddress);

            if (string.IsNullOrWhiteSpace(emailCache))
            {
                var code = new Random().Next(10000, 99999).ToString();
                // 存值,过期时间3分钟
                await _cacheManager.SetAsync(input.EmailAddress, code.ToString(), TimeSpan.FromMinutes(3));
                //await _userEmailer.SendEmailAddressConfirmCode(input.EmailAddress, code.ToString());
            }
            else
            {
                throw new Exception("请检查邮箱中的验证码，有效期3分钟，可能进入垃圾箱了。");
            }
        }


        public async Task<TokenInfoOutput> RefrshToken(RefreshTokenInput input)
        {
            var tokenInfo = await authTokenService.RefreshAuthTokenAsync(input);
            return new TokenInfoOutput()
            {
                AccessToken = tokenInfo.AccessToken,
                RefreshToken = tokenInfo.RefreshToken,
                Expires = tokenInfo.Expiration,
            };
        }
    }
}