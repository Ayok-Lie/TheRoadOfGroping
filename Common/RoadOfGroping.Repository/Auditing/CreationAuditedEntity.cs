using System.ComponentModel.DataAnnotations;
using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.Repository.Auditing
{
    [Serializable]
    public abstract class CreationAuditedEntity : Entity, ICreationAuditedEntity
    {
        /// <summary>
        /// 创建用户
        /// </summary>
        [MaxLength(64)]
        public virtual string? CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreationTime { get; set; }
    }

    [Serializable]
    public abstract class CreationAuditedEntity<Tkey> : Entity<Tkey>, ICreationAuditedEntity
    {
        protected CreationAuditedEntity()
        {
        }

        protected CreationAuditedEntity(Tkey id)
        {
            Id = id;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        [MaxLength(64)]
        public virtual string? CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreationTime { get; set; }
    }
}