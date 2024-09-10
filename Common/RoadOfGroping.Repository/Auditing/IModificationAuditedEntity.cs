using System.ComponentModel.DataAnnotations;

namespace RoadOfGroping.Repository.Auditing
{
    public interface IModificationAuditedEntity
    {
        /// <summary>
        /// 修改人Id
        /// </summary>
        [MaxLength(64)]
        string? ModifierId { get; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? ModificationTime { get; }
    }
}