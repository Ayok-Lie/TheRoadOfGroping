using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.Core.Users.Entity
{
    public class UserRoles : FullAuditedEntity<string>
    {
        [Description("用户Id")]
        [Comment("用户Id")]
        public string UserId { get; set; }

        [Description("角色Id")]
        [Comment("角色Id")]
        public string RoleId { get; set; }
    }
}