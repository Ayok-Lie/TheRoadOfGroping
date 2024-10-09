using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.Provider;

namespace RoadOfGroping.EntityFramework.Extensions
{
    public class DbContextProvider : IDbContextProvider, ITransientDependency
    {
        private readonly RoadOfGropingDbContext _context;

        public DbContextProvider(RoadOfGropingDbContext context)
        {
            _context = context;
        }

        public DbContext GetDbContext()
        {
            return _context;
        }
    }
}