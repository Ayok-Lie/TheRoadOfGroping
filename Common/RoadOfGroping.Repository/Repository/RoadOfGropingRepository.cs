using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.Repository.Repository
{
    public class RoadOfGropingRepository<TDbContext, TEntity> : BaseRepository<TDbContext, TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        public RoadOfGropingRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class RoadOfGropingRepository<TDbContext, TEntity, TKey> : BaseRepository<TDbContext, TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext
    {
        public RoadOfGropingRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }
}