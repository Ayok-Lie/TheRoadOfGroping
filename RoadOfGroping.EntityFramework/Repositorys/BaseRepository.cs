using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.EntityFramework.Domain;

namespace RoadOfGroping.EntityFramework.Repositorys
{
    public abstract class BaseRepository<TEntity, TPrimaryKey> : IBaseRepository<TEntity, TPrimaryKey> where TEntity :
        class, IEntity1<TPrimaryKey>
    {
        private readonly IServiceProvider _serviceProvider;
        protected virtual RoadOfGropingDbContext Context { get; set; }

        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Context = _serviceProvider.GetRequiredService<RoadOfGropingDbContext>();
            DbSet = Context.Set<TEntity>();
        }

        public virtual IServiceProvider ServiceProvider { get; }
        public IQueryable<TEntity> Query => DbSet.AsQueryable();

        public IQueryable<TEntity> QueryAsNoTracking => Query.AsNoTracking();

        public Task BatchDeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(List<TPrimaryKey> idList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindByIdAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AnyAsync(predicate);
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity columns, Expression<Func<TEntity, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }
    }
}