using System.ComponentModel.DataAnnotations;

namespace RoadOfGroping.Repository.Auditing
{
    public interface ICreationAuditedEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime? CreationTime { get; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [MaxLength(64)]
        string CreatorId { get; }
    }
}