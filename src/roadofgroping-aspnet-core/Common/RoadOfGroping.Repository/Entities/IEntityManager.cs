using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Repository.Entities
{
    public interface IEntityManager<TDbContext> : ITransientDependency
        where TDbContext : DbContext
    {
        /// <summary>
        /// 添加种子数据
        /// </summary>
        void DbSeed(Action<TDbContext> action);
    }
}