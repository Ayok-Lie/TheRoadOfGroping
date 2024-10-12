namespace RoadOfGroping.Application.AccountService.Dtos
{
    public class PasswordResetInput
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EmailAddress { get; set; }
    }
}