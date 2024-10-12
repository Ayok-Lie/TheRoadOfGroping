using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Common.Options;
using RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule;
using RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token
{
    public class AuthTokenService : IAuthTokenService
    {
        // 定义刷新令牌的声明类型
        private const string RefreshTokenIdClaimType = "refresh_token_id";

        // 依赖注入的选项配置
        private readonly JwtBearerOptions _jwtBearerOptions;

        private readonly JwtOptions _jwtOptions;
        private readonly CacheManager _distributedCache;
        private readonly ILogger<AuthTokenService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SigningCredentials _signingCredentials;

        // 构造函数，注入依赖
        public AuthTokenService(
           IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
           IOptionsSnapshot<JwtOptions> jwtOptions,
           CacheManager distributedCache,
           ILogger<AuthTokenService> logger,
           IHttpContextAccessor httpContextAccessor,
           SigningCredentials signingCredentials)
        {
            _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _jwtOptions = jwtOptions.Value;
            _distributedCache = distributedCache;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _signingCredentials = signingCredentials;
        }

        // 创建认证令牌的方法
        public async Task<AuthTokenDto> CreateAuthTokenAsync(UserAuthDto user)
        {
            var result = new AuthTokenDto();

            // 创建刷新令牌
            var (refreshTokenId, refreshToken) = await CreateRefreshTokenAsync(user.Id);
            result.RefreshToken = refreshToken;
            // 创建访问令牌
            result.AccessToken = CreateJwtToken(user, refreshTokenId);

            if (user.IsApiLogin)
            {
                // 将访问令牌放入Cookie中
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(HeaderNames.Authorization, result.AccessToken, new CookieOptions
                {
                    IsEssential = true,
                    MaxAge = TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays),
                    Path = "/",
                    SameSite = SameSiteMode.Lax
                });
            }
            return result;
        }

        // 创建刷新令牌的方法
        private async Task<(string refreshTokenId, string refreshToken)> CreateRefreshTokenAsync(string userId)
        {
            var tokenId = Guid.NewGuid().ToString("N");

            // 生成随机字节数组
            var rnBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rnBytes);
            var token = Convert.ToBase64String(rnBytes);

            // 将刷新令牌存储在分布式缓存中
            await _distributedCache.SetAsync(GetRefreshTokenKey(userId, tokenId), token, TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays));

            return (tokenId, token);
        }

        // 创建JWT令牌的方法
        private string CreateJwtToken(UserAuthDto user, string refreshTokenId)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            // 创建令牌描述符
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, string.Join(",", user.Roles)),
                    new Claim(RefreshTokenIdClaimType, refreshTokenId)
                }),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiresMinutes),
                SigningCredentials = _signingCredentials
            };

            // 创建JWT令牌
            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);

            return token;
        }

        // 刷新认证令牌的方法
        public async Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token)
        {
            var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            try
            {
                // 验证访问令牌
                principal = handler.ValidateToken(token.AccessToken, validationParameters, out _);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                throw new BadHttpRequestException("Invalid access token");
            }

            var identity = principal.Identities.First();
            var userId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            var refreshTokenId = identity.Claims.FirstOrDefault(c => c.Type == RefreshTokenIdClaimType).Value;
            var refreshTokenKey = GetRefreshTokenKey(userId, refreshTokenId);
            var refreshToken = await _distributedCache.GetAsync(refreshTokenKey);
            if (refreshToken != token.RefreshToken)
            {
                throw new BadHttpRequestException("Invalid refresh token");
            }

            // 删除旧的刷新令牌
            await _distributedCache.DelAsync(refreshTokenKey);

            // 从数据库中获取用户信息
            var user = new UserAuthDto()
            {
                Id = userId,
                UserName = principal.Identity.Name,
                Roles = role.Split(',').ToList(),
                IsApiLogin = false
            };

            // 创建新的认证令牌
            return await CreateAuthTokenAsync(user);
        }

        // 获取刷新令牌的键
        private string GetRefreshTokenKey(string userId, string refreshTokenId)
        {
            if (userId.IsNullOrEmpty()) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            return $"{userId}:{refreshTokenId}";
        }
    }
}