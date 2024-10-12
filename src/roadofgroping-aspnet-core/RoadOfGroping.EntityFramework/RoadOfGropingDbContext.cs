using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RoadOfGroping.Core.AppSettings.Entitys;
using RoadOfGroping.Core.Files.Entitys;
using RoadOfGroping.Core.OrderTest.Entity;
using RoadOfGroping.Core.Permissions.Entity;
using RoadOfGroping.Core.Roles.Entity;
using RoadOfGroping.Core.Users.Entity;
using RoadOfGroping.EntityFramework.Extensions;
using RoadOfGroping.Repository.Auditing;

namespace RoadOfGroping.EntityFramework
{
    /// <summary>
    /// RoadOfGroping 数据库上下文类。
    /// </summary>
    public class RoadOfGropingDbContext : DbContext
    {
        /// <summary>
        /// 初始化 RoadOfGropingDbContext 类的新实例。
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public RoadOfGropingDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// 订单实体集。
        /// </summary>
        public virtual DbSet<Order> Order { get; set; }

        /// <summary>
        /// 用户实体集。
        /// </summary>
        public virtual DbSet<Users> Users { get; set; }

        public virtual DbSet<FileInfos> FileInfos { get; set; }

        public virtual DbSet<AppSetting> AppSetting { get; set; }

        public virtual DbSet<UserRoles> UserRoles { get; set; }

        public virtual DbSet<Roles> Roles { get; set; }

        public virtual DbSet<PermissionOriginal> PermissionOriginal { get; set; }

        public virtual DbSet<PermissionRoleRelation> PermissionRoleRelation { get; set; }

        /// <summary>
        /// 配置模型创建。
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            OnModelCreatingConfigureGlobalFilters(modelBuilder);
            modelBuilder.ConfigureModel();
        }

        /// <summary>
        /// 异步保存更改。
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">是否在成功保存更改后接受所有更改</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// 是否启用软删除过滤器。
        /// </summary>
        protected virtual bool IsSoftDeleteFilterEnabled => true;

        /// <summary>
        /// 配置全局过滤器。
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected virtual void OnModelCreatingConfigureGlobalFilters(ModelBuilder modelBuilder)
        {
            var methodInfo = GetType().GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                methodInfo!.MakeGenericMethod(entityType.ClrType).Invoke(this, new object?[]
                {
                    modelBuilder, entityType
                });
            }
        }

        /// <summary>
        /// 配置全局过滤器。
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="modelBuilder">模型构建器</param>
        /// <param name="mutableEntityType">可变的实体类型</param>
        protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (mutableEntityType.BaseType == null)
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
            }
        }

        /// <summary>
        /// 创建过滤表达式，用于软删除过滤。
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>过滤表达式</returns>
        protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>>? expression = null;

            if (typeof(IDeletionAuditedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                expression = entity => !IsSoftDeleteFilterEnabled || !EF.Property<bool>(entity, nameof(IDeletionAuditedEntity.IsDeleted));
            }
            return expression;
        }
    }
}