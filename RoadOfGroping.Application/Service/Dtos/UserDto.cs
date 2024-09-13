using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RoadOfGroping.Application.Service.Dtos
{
    public class UserDto
    {
        public Guid? Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码哈希值
        /// </summary>
        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string UserPhone { get; set; }

        public IFormFile File { get; set; }
    }
}