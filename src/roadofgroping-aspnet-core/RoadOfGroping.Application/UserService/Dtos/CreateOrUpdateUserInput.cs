using System.ComponentModel.DataAnnotations;

namespace RoadOfGroping.Application.UserService.Dtos
{
    public class CreateOrUpdateUserInput
    {
        public string? Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码哈希值
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string UserPhone { get; set; }

        public string? Avatar { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string>? Roles { get; set; }
    }
}