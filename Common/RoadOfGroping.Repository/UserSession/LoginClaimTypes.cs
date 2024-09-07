using System.Security.Claims;

namespace RoadOfGroping.Repository.UserSession
{
    /// <summary>
    /// 登录声明类型帮助类，定义了常用的声明类型名称。
    /// </summary>
    public static class LoginClaimTypes
    {
        /// <summary>
        /// 用户名声明类型，默认值为 <see cref="ClaimTypes.Name"/>。
        /// </summary>
        public static string UserName { get; set; } = ClaimTypes.Name;

        /// <summary>
        /// 过期时间声明类型，默认值为 <see cref="ClaimTypes.Expiration"/>。
        /// </summary>
        public static string Expiration { get; set; } = ClaimTypes.Expiration;

        /// <summary>
        /// 用户 ID 声明类型，默认值为 <see cref="ClaimTypes.NameIdentifier"/>。
        /// </summary>
        public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        /// 角色声明类型，默认值为 <see cref="ClaimTypes.Role"/>。
        /// </summary>
        public static string Role { get; set; } = ClaimTypes.Role;
    }
}