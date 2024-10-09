namespace RoadOfGroping.Core.Users.Dtos
{
    public class UserLoginDto
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string Avater { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新Token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime Expires { get; set; }
    }
}