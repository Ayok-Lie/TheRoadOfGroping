namespace RoadOfGroping.Repository.Auditing
{
    public interface IDeletionAuditedEntity
    {
        /// <summary>
        /// 删除人Id
        /// </summary>
        string? DeleterId { get; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeletionTime { get; }

        /// <summary>
        /// 软删除状态
        /// </summary>
        bool? IsDeleted { get; }
    }
}