using System.ComponentModel.DataAnnotations;

namespace RoadOfGroping.Repository.Auditing
{
    public abstract class FullAuditedEntity : CreationAuditedEntity, IDeletionAuditedEntity, IModificationAuditedEntity
    {
        [MaxLength(32)]
        public virtual string? DeleterId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }

        public virtual bool? IsDeleted { get; set; }

        public string? ModifierId { get; set; }

        public DateTime? ModificationTime { get; set; }
    }

    public abstract class FullAuditedEntity<Tkey> : CreationAuditedEntity<Tkey>, IDeletionAuditedEntity, IModificationAuditedEntity
    {
        public FullAuditedEntity()
        {
        }

        public FullAuditedEntity(Tkey id)
        {
            Id = id;
        }

        [MaxLength(32)]
        public virtual string? DeleterId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }

        public virtual bool? IsDeleted { get; set; }

        public string? ModifierId { get; set; }

        public DateTime? ModificationTime { get; set; }
    }
}