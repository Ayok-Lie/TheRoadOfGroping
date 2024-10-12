using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Roles.Entity
{
    public class Roles : FullAuditedEntity<string>
    {
        [MaxLength(256)]
        [Description("角色名称")]
        [Comment("角色名称")]
        public string RoleName { get; set; }

        [MaxLength(256)]
        [Description("角色代码")]
        [Comment("角色代码")]
        public string RoleCode { get; set; }
    }
}