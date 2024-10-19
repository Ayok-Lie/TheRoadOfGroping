namespace RoadOfGroping.Application.AccountService.Dtos
{
    public class TokenInfoOutput
    {
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