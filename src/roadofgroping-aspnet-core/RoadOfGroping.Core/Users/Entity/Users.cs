using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Users.Entity
{
    public class Users : FullAuditedEntity<string>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(256)]
        [Comment("用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [MaxLength(256)]
        [Comment("昵称")]
        public string? NickName { get; set; }

        /// <summary>
        /// 密码哈希值
        /// </summary>
        [Required]
        [Comment("密码哈希值")]
        public string Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(256)]
        [Comment("邮箱")]
        public string? UserEmail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(16)]
        [Comment("手机号")]
        public string UserPhone { get; set; }

        /// <summary>
        /// 头像
        /// </summary>

        public string? Avater { get; set; }
    }
}