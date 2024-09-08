using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Application.Service.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
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

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }
    }
}
