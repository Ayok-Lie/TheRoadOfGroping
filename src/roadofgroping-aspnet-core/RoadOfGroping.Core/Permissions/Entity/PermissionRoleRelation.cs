using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Permissions.Entity
{
    public class PermissionRoleRelation : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Comment("角色Id")]
        [Required]
        [MaxLength(64)]
        public string RoleId { get; set; }

        /// <summary>
        /// 权限Id
        /// </summary>
        [Comment("权限Id")]
        [Required]
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 是否授权
        /// </summary>
        [Required]
        [Comment("是否授权")]
        public bool IsGranted { get; set; }
    }
}