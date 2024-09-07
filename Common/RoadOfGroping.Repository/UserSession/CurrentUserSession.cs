using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Repository.UserSession
{
    public class CurrentUserSession : IUserSession, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 用户id
        /// </summary>
        public virtual string UserId => FindClaim(LoginClaimTypes.UserId)?.Value ?? string.Empty;

        /// <summary>
        /// 用户名称
        /// </summary>
        public virtual string UserName => FindClaim(LoginClaimTypes.UserName)?.Value ?? string.Empty;

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsAdmin => UserId == "45D6422E-0EBB-45DB-DC2A-08DC86A36122";

        //public virtual IEnumerable<string> RoleName => FindClaims(LoginClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

        public virtual Claim FindClaim(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == claimType);
        }

        public virtual Claim[] FindClaims(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.Claims.Where(c => c.Type == claimType).ToArray() ?? new Claim[0];
        }
    }
}