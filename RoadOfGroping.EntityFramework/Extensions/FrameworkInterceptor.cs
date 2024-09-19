using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.EntityFramework.Extensions
{
    public class FrameworkInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateTimestamps(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateTimestamps(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                var entityType = entry.Entity.GetType();
                var baseType = entityType.BaseType;

                // 检查是否为FullAuditedEntity的子类
                if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(FullAuditedEntity<>))
                {
                    if (entry.State == EntityState.Added)
                    {
                        // 设置ID和创建时间
                        SetPropertyValue(baseType, entry.Entity, nameof(FullAuditedEntity<string>.Id), Guid.NewGuid().ToString("N"));
                        SetPropertyValue(baseType, entry.Entity, nameof(FullAuditedEntity<string>.CreationTime), DateTime.UtcNow);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        // 设置修改时间
                        SetPropertyValue(baseType, entry.Entity, nameof(FullAuditedEntity<string>.ModificationTime), DateTime.UtcNow);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        // 设置删除时间和IsDeleted标记
                        SetPropertyValue(baseType, entry.Entity, nameof(FullAuditedEntity<string>.DeletionTime), DateTime.UtcNow);
                        SetPropertyValue(baseType, entry.Entity, nameof(FullAuditedEntity<bool>.IsDeleted), true);
                    }
                }
                // 如果不是FullAuditedEntity的子类，直接进行保存，不做任何处理
            }
        }

        private void SetPropertyValue(Type baseType, object entity, string propertyName, object value)
        {
            var property = baseType.GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, value);
            }
        }
    }
}