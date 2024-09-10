using Microsoft.EntityFrameworkCore;
using RoadOfGroping.EntityFramework.Domain;

namespace RoadOfGroping.EntityFramework.Repositorys
{
    public abstract class Repository<TEntity, TDbContext> : IRepository<TEntity> where TEntity : Entity1, IAggregateRoot where TDbContext : RoadOfGropingDbContext
    {
        protected virtual TDbContext DbContext { get; set; }

        public Repository(TDbContext context)
        {
            this.DbContext = context;
        }

        public virtual TEntity Add(TEntity entity)
        {
            return DbContext.Add(entity).Entity;
        }

        public virtual Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Add(entity));
        }

        public virtual TEntity Update(TEntity entity)
        {
            return DbContext.Update(entity).Entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity));
        }

        public virtual bool Remove(Entity1 entity)
        {
            DbContext.Remove(entity);
            return true;
        }

        public virtual Task<bool> RemoveAsync(Entity1 entity)
        {
            return Task.FromResult(Remove(entity));
        }
    }

    public abstract class Repository<TEntity, TKey, TDbContext> : Repository<TEntity, TDbContext>, IRepository<TEntity, TKey> where TEntity : Entity1<TKey>, IAggregateRoot where TDbContext : RoadOfGropingDbContext
    {
        protected readonly DbSet<TEntity> DbSet;

        public Repository(TDbContext context) : base(context)
        {
            DbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Query => DbSet.AsQueryable();

        public virtual bool Delete(TKey id)
        {
            var entity = DbContext.Find<TEntity>(id);
            if (entity == null)
            {
                return false;
            }
            DbContext.Remove(entity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbContext.FindAsync<TEntity>(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }
            DbContext.Remove(entity);
            return true;
        }

        public virtual TEntity Get(TKey id)
        {
            return DbContext.Find<TEntity>(id);
        }

        public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await DbContext.FindAsync<TEntity>(id, cancellationToken);
        }
    }
}