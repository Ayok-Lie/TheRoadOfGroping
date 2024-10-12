using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Permissions.Entity
{
    public class PermissionOriginal : FullAuditedEntity<Guid>
    {
        [Description("权限名称")]
        [Comment("权限名称")]
        public string DisplayName { get; set; }

        [Description("权限代码")]
        [Comment("权限代码")]
        [MaxLength(128)]
        public string Code { get; set; }

        [Description("父权限代码")]
        [Comment("父权限代码")]
        [MaxLength(128)]
        public string? ParentCode { get; set; }

        [Description("排序")]
        [Comment("排序")]
        public int Sort { get; set; }

        [Description("是否系统权限")]
        [Comment("是否系统权限")]
        public bool IsSystem { get; set; }
    }
}