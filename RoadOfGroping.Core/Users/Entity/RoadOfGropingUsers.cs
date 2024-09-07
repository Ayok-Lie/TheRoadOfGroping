using System.ComponentModel.DataAnnotations;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Users.Entity
{
    public class RoadOfGropingUsers : FullAuditedEntity<Guid>
    {
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