using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacDemo.Modules;

namespace AutoFacDemo.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UserAutoFac(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacModule()));
        }
    }
}