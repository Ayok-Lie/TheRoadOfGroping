using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RoadOfGroping.Model
{
    public class ServiceConfigerContext
    {
        public IServiceCollection Services { get; private set; }

        public IServiceProvider Provider
        {
            get
            {
                if (Services is null)
                {
                    throw new ArgumentNullException(nameof(Services) + "ServiceConfigerContext中Service为空");
                }
                return Services.BuildServiceProvider();
            }
        }

        public ServiceConfigerContext(IServiceCollection services)
        {
            Services = services;
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}