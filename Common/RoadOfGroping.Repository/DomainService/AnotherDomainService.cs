using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Repository;

namespace RoadOfGroping.Repository.DomainService
{
    public abstract class AnotherDomainService<TEntity, TPrimaryKey> : IAnotherDomainService<TEntity, TPrimaryKey>, ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        public virtual IServiceProvider ServiceProvider { get; private set; }
        public IAnotherBaseRepository<TEntity, TPrimaryKey> Repo { get; }
        public IQueryable<TEntity> Query => Repo.GetAll();
        public IQueryable<TEntity> QueryAsNoTracking => Query.AsNoTracking();

        public AnotherDomainService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Repo = serviceProvider.GetRequiredService<IAnotherBaseRepository<TEntity, TPrimaryKey>>();
        }

        public abstract IQueryable<TEntity> GetIncludeQuery();

        public abstract Task ValidateOnDelete(TEntity entity);

        public abstract Task ValidateOnCreateOrUpdate(TEntity entity);

        public async Task<TEntity> FindByIdWithInclude(TPrimaryKey id)
        {
            return await GetIncludeQuery().FirstOrDefaultAsync((TEntity x) => x.Id.Equals(id));
        }

        public async Task<TEntity> FindById(TPrimaryKey id)
        {
            return await QueryAsNoTracking.FirstOrDefaultAsync((TEntity x) => x.Id.Equals(id));
        }

        public async Task<TEntity> IsExist(TPrimaryKey id)
        {
            TEntity val = await FindById(id);
            if (val == null)
            {
                throw new Exception("Error: NullError" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return val;
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            await Repo.InsertAsync(entity);
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            await Repo.UpdateAsync(entity);
            return entity;
        }

        public async Task Delete(TPrimaryKey id)
        {
            await Delete(await FindByIdWithInclude(id));
        }

        public async Task Delete(TEntity entity)
        {
            await ValidateOnDelete(entity);
            ClearNavigationProp(entity);
            await Repo.DeleteAsync(entity);
        }

        //
        // 摘要:
        //     清空导航属性
        //
        // 参数:
        //   obj:
        protected virtual void ClearNavigationProp(object obj)
        {
            if (obj == null)
            {
                return;
            }

            //PropertyInfo[] properties = obj.GetType().GetProperties();
            //foreach (PropertyInfo propertyInfo in properties)
            //{
            //    if (!propertyInfo.PropertyType.IsStructs())
            //    {
            //        propertyInfo.SetValue(obj, null);
            //    }
            //}
        }
    }
}