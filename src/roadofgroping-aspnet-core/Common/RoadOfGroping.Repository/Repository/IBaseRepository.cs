using System.Linq.Expressions;

namespace RoadOfGroping.Repository.Repository
{
    /// <summary>
    /// 基础仓储接口，定义了对实体的基本操作。
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 异步添加实体。
        /// </summary>
        /// <param name="entity">要添加的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>添加的实体</returns>
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 同步添加实体。
        /// </summary>
        /// <param name="entity">要添加的实体</param>
        /// <returns>添加的实体</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 异步添加多个实体。
        /// </summary>
        /// <param name="entities">要添加的实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步修改实体。
        /// </summary>
        /// <param name="entity">要修改的实体</param>
        /// <returns>修改后的实体</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// 异步修改多个实体。
        /// </summary>
        /// <param name="entities">要修改的实体集合</param>
        /// <returns>任务</returns>
        Task UpdateManyAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步删除实体。
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步删除满足条件的实体。
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步删除多个实体。
        /// </summary>
        /// <param name="entities">要删除的实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有实体的 IQueryable 对象。
        /// </summary>
        /// <returns>IQueryable 对象</returns>
        IQueryable<TEntity> GetQueryAll();

        /// <summary>
        /// 异步添加或修改实体。
        /// </summary>
        /// <param name="predicate">判断条件</param>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        Task<TEntity> InsertOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity);

        /// <summary>
        /// 异步获取满足条件的实体列表。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步获取实体总数。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体总数</returns>
        Task<long> GetCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步获取满足条件的实体数量。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体数量</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步获取分页列表。
        /// </summary>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页列表和总记录数</returns>
        Task<(List<TEntity>, int)> GetPagedListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步获取满足条件的第一个实体，不存在则返回 null。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步获取满足条件的第一个实体，不存在则抛出异常。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步判断是否存在满足条件的实体。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否存在</returns>
        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步获取满足条件的 IQueryable 对象。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>IQueryable 对象</returns>
        Task<IQueryable<TEntity>> GetQueryAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步获取满足条件并选择指定字段的 IQueryable 对象。
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="selector">选择器</param>
        /// <returns>IQueryable 对象</returns>
        Task<IQueryable<TResult>> GetQueryAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
    }

    /// <summary>
    /// 基础仓储接口，定义了对实体的基本操作，包含主键类型。
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public interface IBaseRepository<TEntity, TPrimaryKey> : IBaseRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 异步删除指定主键的实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task DeleteIDAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步删除多个指定主键的实体。
        /// </summary>
        /// <param name="ids">主键集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task DeleteManyAsync(IEnumerable<TPrimaryKey> ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步删除所有实体。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task DeleteManyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步获取指定主键的实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="includeDetails">是否包含详细信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        Task<TEntity> GetAsync(TPrimaryKey id, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步查找指定主键的实体，不存在则返回 null。
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="includeDetails">是否包含详细信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        Task<TEntity> FindAsync(TPrimaryKey id, bool includeDetails = true, CancellationToken cancellationToken = default);
    }
}