using System.Linq.Expressions;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Repository;

namespace RoadOfGroping.Repository.DomainService
{
    /// <summary>
    /// 基础领域服务接口，定义了对实体的基本操作。
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public interface IBasicDomainService<TEntity, TPrimaryKey> : ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 实体仓储接口。
        /// </summary>
        IBaseRepository<TEntity, TPrimaryKey> EntityRepo { get; }

        /// <summary>
        /// 查询器，用于查询实体。
        /// </summary>
        IQueryable<TEntity> Query { get; }

        /// <summary>
        /// 查询器，用于查询实体，不追踪实体状态。
        /// </summary>
        IQueryable<TEntity> QueryAsNoTracking { get; }

        /// <summary>
        /// 根据主键异步查找实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        Task<TEntity> FindByIdAsync(TPrimaryKey id);

        /// <summary>
        /// 异步创建实体。
        /// </summary>
        /// <param name="entity">要创建的实体</param>
        /// <returns>创建的实体</returns>
        Task<TEntity> CreateAsync(TEntity entity);

        /// <summary>
        /// 异步批量创建实体。
        /// </summary>
        /// <param name="entities">要创建的实体集合</param>
        /// <returns>任务</returns>
        Task CreateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步更新实体。
        /// </summary>
        /// <param name="entity">要更新的实体</param>
        /// <returns>更新的实体</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// 异步批量更新实体。
        /// </summary>
        /// <param name="entities">要更新的实体集合</param>
        /// <returns>任务</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步根据条件更新实体。
        /// </summary>
        /// <param name="columns">要更新的字段</param>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>更新的实体</returns>
        Task<TEntity> UpdateAsync(TEntity columns, Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 异步删除指定主键的实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>任务</returns>
        Task DeleteAsync(TPrimaryKey id);

        /// <summary>
        /// 异步删除实体。
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <returns>任务</returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// 异步根据条件删除实体。
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步批量删除指定主键的实体。
        /// </summary>
        /// <param name="idList">主键列表</param>
        /// <returns>任务</returns>
        Task DeleteAsync(List<TPrimaryKey> idList);

        /// <summary>
        /// 异步批量删除所有实体。
        /// </summary>
        /// <returns>任务</returns>
        Task BatchDeleteAsync();

        /// <summary>
        /// 异步判断指定主键的实体是否存在。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistAsync(TPrimaryKey id);

        /// <summary>
        /// 异步创建或更新实体。
        /// </summary>
        /// <param name="predicate">判断条件</param>
        /// <param name="entity">实体</param>
        /// <returns>实体</returns>
        Task<TEntity> CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity);

        /// <summary>
        /// 异步批量创建或更新实体。
        /// </summary>
        /// <param name="predicate">判断条件</param>
        /// <param name="entities">实体集合</param>
        /// <returns>任务</returns>
        Task CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步判断是否存在满足条件的实体。
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>是否存在</returns>
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate);
    }
}