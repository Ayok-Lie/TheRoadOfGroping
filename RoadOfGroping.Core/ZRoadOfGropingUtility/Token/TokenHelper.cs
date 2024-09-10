using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RoadOfGroping.Common.JWTHelpers;
using RoadOfGroping.Core.OrderTest.Entity;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token
{
    public class TokenHelper
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly ILogger<TokenHelper> _logger;
        private IJwtModel jwtModel;

        public TokenHelper(
            IConfiguration configuration,
            JwtSecurityTokenHandler jwtSecurityTokenHandler,
            ILogger<TokenHelper> logger
            )
        {
            _configuration = configuration;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _logger = logger;
            jwtModel = _configuration.GetSection("JWT").Get<JwtModel>();
        }

        // <summary>
        /// 创建加密JwtToken
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string CreateJwtToken()
        {
            var order = new Order();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtModel.SecretKey));

            return JwtHelper.CreateJwtToken(order, jwtModel, secretKey);
        }

        public Order GetToken(string Token)
        {
            return JwtHelper.GetToken<Order>(Token);
        }

        public bool VerTokenAsync(string Token)
        {
            //业务逻辑暂时没有写
            return true;
        }

        /// <summary>
        /// 根据一个对象通过反射提供负载，生成token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        public string CreateFromObjectToken<T>(T entity) where T : class
        {
            //定义声明的集合
            List<Claim> claims = new List<Claim>();

            //用反射把数据提供给它
            foreach (var item in entity.GetType().GetProperties())
            {
                object obj = item.GetValue(entity);
                string value = "";
                if (obj != null)
                {
                    value = obj.ToString();
                }

                claims.Add(new Claim(item.Name, value));
            }

            //根据声明 生成token字符串
            return CreateTokenString(claims);
        }

        /// <summary>
        /// 根据键值对提供负载，生成token
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public string CreateToken(List<Claim> claims)
        {
            //根据声明 生成token字符串
            return CreateTokenString(claims);
        }

        /// <summary>
        /// 私有方法，用于生成Token字符串
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string CreateTokenString(List<Claim> claims)
        {
            //过期时间
            DateTime expires = DateTime.Now.AddMinutes(jwtModel.AccessTokenExpiresMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtModel.Issuer,
                audience: jwtModel.Audience,
                claims: claims,           //携带的荷载
                notBefore: DateTime.Now,  //token生成时间
                expires: expires,         //token过期时间
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtModel.SecretKey)), SecurityAlgorithms.HmacSha256
                    )
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}