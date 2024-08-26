using Microsoft.EntityFrameworkCore;
using RoadOfGroping.EntityFramework.Domain;

namespace RoadOfGroping.EntityFramework.Repositorys
{
    public class ZRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity>
        where TEntity : class, IEntity1
        where TDbContext : DbContext
    {
        public ZRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class ZRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext, TEntity, TKey>
        where TEntity : class, IEntity1<TKey>
        where TDbContext : DbContext
    {
        public ZRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }
}