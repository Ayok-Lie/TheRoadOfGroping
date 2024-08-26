using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RoadOfGroping.Repository.Extensions
{
    public static class DispatcherOptionsExtensions
    {
        public static IServiceCollection UseRepository<TDbContext>(
            this IServiceCollection server,
            params Type[] entityTypes)
            where TDbContext : DbContext
            => server.UseRepository<TDbContext>(entityTypes.Length == 0 ? null : (IEnumerable<Type>)entityTypes);

        public static IServiceCollection UseRepository<TDbContext>(
            this IServiceCollection server,
            IEnumerable<Type>? entityTypes)
            where TDbContext : DbContext
        {
            if (server.Any(service => service.ImplementationType == typeof(RepositoryProvider)))
                return server;

            server.AddSingleton<RepositoryProvider>();

            server.TryAddRepository<TDbContext>(GetAssemblies().Distinct(), entityTypes);
            return server;
        }

        private sealed class RepositoryProvider
        {
        }

        public static IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}