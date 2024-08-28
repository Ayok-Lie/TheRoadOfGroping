using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.Repository.Repository
{
    /// <summary>
    /// 通用仓储实现，包含主键类型。
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class BaseRepository<TDbContext, TEntity, TKey> : BaseRepository<TDbContext, TEntity>, IBaseRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext
    {
        protected BaseRepository(TDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 异步删除指定主键的实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task DeleteIDAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken: cancellationToken);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        /// <summary>
        /// 异步删除多个指定主键的实体。
        /// </summary>
        /// <param name="ids">主键集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task DeleteManyAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            var entities = await DbSet.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
            if (entities.Count > 0)
            {
                DbSet.RemoveRange(entities);
            }
        }

        /// <summary>
        /// 异步查找指定主键的实体，不存在则返回 null。
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="includeDetails">是否包含详细信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        public async Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
            => await DbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken: cancellationToken);

        /// <summary>
        /// 异步获取指定主键的实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="includeDetails">是否包含详细信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        public async Task<TEntity?> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
            => await DbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 通用仓储实现，不包含主键类型。
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public abstract class BaseRepository<TDbContext, TEntity> : IBaseRepository<TEntity>, ITransientDependency
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(TDbContext dbContext)
        {
            DbSet = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// 异步获取满足条件的第一个实体，不存在则返回 null。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 异步获取满足条件的第一个实体，不存在则抛出异常。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体</returns>
        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstAsync(predicate, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 异步获取满足条件的实体列表。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 异步判断是否存在满足条件的实体。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.CountAsync(predicate, cancellationToken) > 0;
        }

        /// <summary>
        /// 异步获取满足条件的 IQueryable 对象。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>IQueryable 对象</returns>
        public async Task<IQueryable<TEntity>> GetQueryAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(DbSet.Where(predicate));
        }

        /// <summary>
        /// 异步获取满足条件并选择指定字段的 IQueryable 对象。
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="selector">选择器</param>
        /// <returns>IQueryable 对象</returns>
        public async Task<IQueryable<TResult>> GetQueryAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return await Task.FromResult(DbSet.Where(predicate).Select(selector));
        }

        /// <summary>
        /// 异步添加实体。
        /// </summary>
        /// <param name="entity">要添加的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>添加的实体</returns>
        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return (await DbSet.AddAsync(entity, cancellationToken)).Entity;
        }

        /// <summary>
        /// 同步添加实体。
        /// </summary>
        /// <param name="entity">要添加的实体</param>
        /// <returns>添加的实体</returns>
        public TEntity Insert(TEntity entity)
        {
            return DbSet.Add(entity).Entity;
        }

        /// <summary>
        /// 异步添加多个实体。
        /// </summary>
        /// <param name="entities">要添加的实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// 异步删除实体。
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步删除多个实体。
        /// </summary>
        /// <param name="entities">要删除的实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            DbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步删除所有实体。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task DeleteManyAsync(CancellationToken cancellationToken = default)
        {
            DbSet.RemoveRange(DbSet);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步修改实体。
        /// </summary>
        /// <param name="entity">要修改的实体</param>
        /// <returns>修改后的实体</returns>
        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
            return Task.FromResult(entity);
        }

        /// <summary>
        /// 异步修改多个实体。
        /// </summary>
        /// <param name="entities">要修改的实体集合</param>
        /// <returns>任务</returns>
        public Task UpdateManyAsync(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步获取所有实体的列表。
        /// </summary>
        /// <param name="includeDetails">是否包含详细信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        public Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步获取实体总数。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体总数</returns>
        public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步获取分页列表。
        /// </summary>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页列表和总记录数</returns>
        public async Task<(List<TEntity>, int)> GetPagedListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            List<TEntity> list = await DbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (list, await DbSet.CountAsync());
        }

        /// <summary>
        /// 异步删除满足条件的实体。
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entityList = await DbSet.Where(predicate).ToListAsync();
            if (entityList != null && entityList.Any())
            {
                DbSet.RemoveRange(entityList);
            }
        }

        /// <summary>
        /// 获取所有实体的 IQueryable 对象。
        /// </summary>
        /// <returns>IQueryable 对象</returns>
        public IQueryable<TEntity> GetQueryAll()
        {
            return DbSet.AsQueryable();
        }

        /// <summary>
        /// 异步添加或修改实体。
        /// </summary>
        /// <param name="predicate">判断条件</param>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        public async Task<TEntity> InsertOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
        {
            var existingEntity = await DbSet.FirstOrDefaultAsync(predicate);
            if (existingEntity == null)
            {
                DbSet.Add(entity); // 如果记录不存在，则新增
            }
            else
            {
                // 如果记录存在，则进行更新
                DbSet.Attach(existingEntity);
                DbSet.Update(entity);
            }

            return await Task.FromResult(entity);
        }

        /// <summary>
        /// 异步获取满足条件的实体数量。
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体数量</returns>
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }
    }
}