using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Interface.DependencyInjection;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Repository;

namespace RoadOfGroping.Repository.DomainService
{
    public abstract class BasicDomainService<TEntity, TPrimaryKey> : IBasicDomainService<TEntity, TPrimaryKey>, ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        public virtual IServiceProvider ServiceProvider { get; }

        //public virtual IAbpSession AbpSession { get; }

        public virtual IBaseRepository<TEntity, TPrimaryKey> EntityRepo { get; }

        public virtual IQueryable<TEntity> Query => EntityRepo.GetQueryAll();

        public virtual IQueryable<TEntity> QueryAsNoTracking => Query.AsNoTracking();

        public BasicDomainService(IServiceProvider serviceProvider) : base()
        {
            ServiceProvider = serviceProvider;
            EntityRepo = serviceProvider.GetRequiredService<IBaseRepository<TEntity, TPrimaryKey>>();
        }

        public abstract Task ValidateOnCreateOrUpdate(TEntity entity);

        public virtual async Task<TEntity> FindByIdAsync(TPrimaryKey id)
        {
            return await EntityRepo.GetAsync(id);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            return await EntityRepo.InsertAsync(entity);
        }

        public virtual async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await QueryAsNoTracking.AnyAsync(predicate);
        }

        public async Task CreateAsync(IEnumerable<TEntity> entities)
        {
            await EntityRepo.InsertManyAsync(entities);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            return await EntityRepo.UpdateAsync(entity);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity columns, Expression<Func<TEntity, bool>> whereExpression)
        {
            var entity = await QueryAsNoTracking.Where(whereExpression).FirstAsync();
            //ObjectMapper.Map(columns, entity);
            CopyNonNullProperties(columns, entity);
            await ValidateOnCreateOrUpdate(entity);
            return await EntityRepo.UpdateAsync(entity);
        }

        private void CopyNonNullProperties<TEntity>(TEntity source, TEntity destination)
        {
            // 获取源和目标类型的所有属性
            PropertyInfo[] sourceProperties = source.GetType().GetProperties();
            PropertyInfo[] destinationProperties = destination.GetType().GetProperties();

            // 遍历源类型的属性
            foreach (var sourceProperty in sourceProperties)
            {
                // 获取属性名称
                string propertyName = sourceProperty.Name;

                // 获取源属性的值
                object sourceValue = sourceProperty.GetValue(source);

                // 查找目标类型中是否存在同名属性
                PropertyInfo destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == propertyName);

                // 如果存在同名属性且源属性的值不为空，则将值复制到目标属性中
                if (destinationProperty != null && sourceValue != null)
                {
                    destinationProperty.SetValue(destination, sourceValue);
                }
            }
        }

        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                await UpdateAsync(entity);
            }
        }

        public virtual async Task DeleteAsync(TPrimaryKey id)
        {
            await EntityRepo.DeleteIDAsync(id);
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await EntityRepo.DeleteAsync(entity);
        }

        public virtual async Task DeleteAsync(List<TPrimaryKey> idList)
        {
            if (idList != null && idList.Count != 0)
            {
                await EntityRepo.DeleteManyAsync(idList);
            }
        }

        public async Task BatchDeleteAsync()
        {
            await EntityRepo.DeleteManyAsync(Query);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await EntityRepo.DeleteAsync(predicate);
        }

        public async Task<bool> ExistAsync(TPrimaryKey id)
        {
            return await EntityRepo.CountAsync((o) => o.Id.Equals(id)) > 0;
        }

        public async Task<TEntity> CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate,
            TEntity entity)
        {
            return await EntityRepo.InsertOrUpdateAsync(predicate, entity);
        }

        public async Task CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                await EntityRepo.InsertOrUpdateAsync(predicate, entity);
            }
        }

        protected virtual void ThrowDeleteError(string def, string defRef1, string defRef2)
        {
            //throw new UserFriendlyException($"错误! {def}【{defRef1}】 被 {defRef2} 引用。  时间：{DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        //
        // 摘要:
        //     抛出 RepetError 异常
        //
        // 参数:
        //   name:
        //     ndo的名称
        protected virtual void ThrowRepetError(string name)
        {
            //throw new UserFriendlyException($"错误! 数据名称【{name}】重复。 时间：{DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        //
        // 摘要:
        //     抛出 ThrowUserFriendlyError 异常
        protected virtual void ThrowUserFriendlyError(string reason)
        {
            //throw new UserFriendlyException($"错误! {reason}。 时间：{DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        //
        // 摘要:
        //     获取服务实例
        //
        // 类型参数:
        //   T:
        protected T GetService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}