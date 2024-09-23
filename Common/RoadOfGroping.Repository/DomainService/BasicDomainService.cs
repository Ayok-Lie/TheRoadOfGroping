using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Repository;

namespace RoadOfGroping.Repository.DomainService
{
    /// <summary>
    /// 基础领域服务实现，定义了对实体的基本操作。
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public abstract class BasicDomainService<TEntity, TPrimaryKey> : ServiceBase, IBasicDomainService<TEntity, TPrimaryKey>, ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 服务提供者。
        /// </summary>
        public virtual IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 日志记录器。
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// 实体仓储接口。
        /// </summary>
        public virtual IBaseRepository<TEntity, TPrimaryKey> EntityRepo { get; }

        /// <summary>
        /// 查询器，用于查询实体。
        /// </summary>
        public virtual IQueryable<TEntity> Query => EntityRepo.GetQueryAll();

        /// <summary>
        /// 查询器，用于查询实体，不追踪实体状态。
        /// </summary>
        public virtual IQueryable<TEntity> QueryAsNoTracking => Query.AsNoTracking();

        /// <summary>
        /// 初始化基础领域服务实例。
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public BasicDomainService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ServiceProvider = serviceProvider;
            EntityRepo = serviceProvider.GetRequiredService<IBaseRepository<TEntity, TPrimaryKey>>();
            Logger = serviceProvider.GetRequiredService<ILogger<BasicDomainService<TEntity, TPrimaryKey>>>();
        }

        /// <summary>
        /// 在创建或更新实体时进行验证。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        public abstract Task ValidateOnCreateOrUpdate(TEntity entity);

        /// <summary>
        /// 根据主键异步查找实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public virtual async Task<TEntity> FindByIdAsync(TPrimaryKey id)
        {
            try
            {
                return await EntityRepo.GetAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error finding entity by id {id}");
                throw;
            }
        }

        /// <summary>
        /// 异步创建实体。
        /// </summary>
        /// <param name="entity">要创建的实体</param>
        /// <returns>创建的实体</returns>
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                await ValidateOnCreateOrUpdate(entity);
                return await EntityRepo.InsertAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating entity {entity}");
                throw;
            }
        }

        /// <summary>
        /// 异步判断是否存在满足条件的实体。
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>是否存在</returns>
        public virtual async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await QueryAsNoTracking.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error checking if any entity exists with predicate {predicate}");
                throw;
            }
        }

        /// <summary>
        /// 异步批量创建实体。
        /// </summary>
        /// <param name="entities">要创建的实体集合</param>
        /// <returns>任务</returns>
        public async Task CreateAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await EntityRepo.InsertManyAsync(entities);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating entities {entities}");
                throw;
            }
        }

        /// <summary>
        /// 异步更新实体。
        /// </summary>
        /// <param name="entity">要更新的实体</param>
        /// <returns>更新的实体</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            try
            {
                await ValidateOnCreateOrUpdate(entity);
                return await EntityRepo.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating entity {entity}");
                throw;
            }
        }

        /// <summary>
        /// 异步根据条件更新实体。
        /// </summary>
        /// <param name="columns">要更新的字段</param>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>更新的实体</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity columns, Expression<Func<TEntity, bool>> whereExpression)
        {
            try
            {
                var entity = await QueryAsNoTracking.Where(whereExpression).FirstAsync();
                CopyNonNullProperties(columns, entity);
                await ValidateOnCreateOrUpdate(entity);
                return await EntityRepo.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating entity with columns {columns} and where expression {whereExpression}");
                throw;
            }
        }

        /// <summary>
        /// 复制非空属性。
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">源实体</param>
        /// <param name="destination">目标实体</param>
        private void CopyNonNullProperties<TEntity>(TEntity source, TEntity destination)
        {
            PropertyInfo[] sourceProperties = source.GetType().GetProperties();
            PropertyInfo[] destinationProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                string propertyName = sourceProperty.Name;
                object sourceValue = sourceProperty.GetValue(source);
                PropertyInfo destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == propertyName);

                if (destinationProperty != null && sourceValue != null)
                {
                    destinationProperty.SetValue(destination, sourceValue);
                }
            }
        }

        /// <summary>
        /// 异步批量更新实体。
        /// </summary>
        /// <param name="entities">要更新的实体集合</param>
        /// <returns>任务</returns>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (TEntity entity in entities)
                {
                    await UpdateAsync(entity);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating entities {entities}");
                throw;
            }
        }

        /// <summary>
        /// 异步删除指定主键的实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>任务</returns>
        public virtual async Task DeleteAsync(TPrimaryKey id)
        {
            try
            {
                await EntityRepo.DeleteIDAsync(id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting entity by id {id}");
                throw;
            }
        }

        /// <summary>
        /// 异步删除实体。
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <returns>任务</returns>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            try
            {
                await EntityRepo.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting entity {entity}");
                throw;
            }
        }

        /// <summary>
        /// 异步批量删除指定主键的实体。
        /// </summary>
        /// <param name="idList">主键列表</param>
        /// <returns>任务</returns>
        public virtual async Task DeleteAsync(List<TPrimaryKey> idList)
        {
            try
            {
                if (idList != null && idList.Count != 0)
                {
                    await EntityRepo.DeleteManyAsync(idList);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting entities by id list {idList}");
                throw;
            }
        }

        /// <summary>
        /// 异步批量删除所有实体。
        /// </summary>
        /// <returns>任务</returns>
        public async Task BatchDeleteAsync()
        {
            try
            {
                await EntityRepo.DeleteManyAsync(Query);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error batch deleting entities");
                throw;
            }
        }

        /// <summary>
        /// 异步根据条件删除实体。
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                await EntityRepo.DeleteAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting entities by predicate {predicate}");
                throw;
            }
        }

        /// <summary>
        /// 异步判断指定主键的实体是否存在。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistAsync(TPrimaryKey id)
        {
            try
            {
                return await EntityRepo.CountAsync(o => o.Id.Equals(id)) > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error checking if entity exists by id {id}");
                throw;
            }
        }

        /// <summary>
        /// 异步创建或更新实体。
        /// </summary>
        /// <param name="predicate">判断条件</param>
        /// <param name="entity">实体</param>
        /// <returns>实体</returns>
        public async Task<TEntity> CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
        {
            try
            {
                return await EntityRepo.InsertOrUpdateAsync(predicate, entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating or updating entity {entity} with predicate {predicate}");
                throw;
            }
        }

        /// <summary>
        /// 异步批量创建或更新实体。
        /// </summary>
        /// <param name="predicate">判断条件</param>
        /// <param name="entities">实体集合</param>
        /// <returns>任务</returns>
        public async Task CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (TEntity entity in entities)
                {
                    await EntityRepo.InsertOrUpdateAsync(predicate, entity);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating or updating entities {entities} with predicate {predicate}");
                throw;
            }
        }

        /// <summary>
        /// 抛出删除错误异常。
        /// </summary>
        /// <param name="def">定义</param>
        /// <param name="defRef1">引用1</param>
        /// <param name="defRef2">引用2</param>
        protected virtual void ThrowDeleteError(string def, string defRef1, string defRef2)
        {
            //throw new UserFriendlyException($"错误! {def}【{defRef1}】 被 {defRef2} 引用。  时间：{DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        /// <summary>
        /// 抛出重复错误异常。
        /// </summary>
        /// <param name="name">名称</param>
        protected virtual void ThrowRepetError(string name)
        {
            //throw new UserFriendlyException($"错误! 数据名称【{name}】重复。 时间：{DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        /// <summary>
        /// 抛出用户友好错误异常。
        /// </summary>
        /// <param name="reason">原因</param>
        protected virtual void ThrowUserFriendlyError(string reason)
        {
            //throw new UserFriendlyException($"错误! {reason}。 时间：{DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        /// <summary>
        /// 获取服务实例。
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        protected T GetService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}