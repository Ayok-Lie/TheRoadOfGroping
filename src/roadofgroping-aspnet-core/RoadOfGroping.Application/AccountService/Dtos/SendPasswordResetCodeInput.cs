using System.ComponentModel.DataAnnotations;

namespace RoadOfGroping.Application.AccountService.Dtos
{
    public class SendPasswordResetCodeInput
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required(ErrorMessage = "该字段为必填项")]
        [EmailAddress(ErrorMessage = "请输入有效的电子邮件地址")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerificationCode { get; set; }
    }
}