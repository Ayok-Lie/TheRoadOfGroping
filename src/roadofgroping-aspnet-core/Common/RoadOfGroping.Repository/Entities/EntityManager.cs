using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Repository.Entities
{
    public class EntityManager<TDbContext> : IEntityManager<TDbContext>, ITransientDependency
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityManager(TDbContext dbContext, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void DbSeed(Action<TDbContext> action)
        {
            using var scope = _serviceProvider.CreateScope();
            var _dbcontext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            action.Invoke(_dbcontext);
        }
    }
}