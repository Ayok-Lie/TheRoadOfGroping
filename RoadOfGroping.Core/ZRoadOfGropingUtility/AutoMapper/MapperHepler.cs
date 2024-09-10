using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.AutoMapper
{
    public static class MapperHepler
    {
        public static Mapper CreateMappings(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<AutoMapperOptions>>().Value;

                void ConfigureAll(IAutoMapperConfigurationContext ctx)
                {
                    foreach (var configurator in options.Configurators)
                    {
                        configurator(ctx);
                    }
                }

                options.Configurators.Insert(0, ctx => ctx.MapperConfiguration.ConstructServicesUsing(serviceProvider.GetService));

                var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
                {
                    ConfigureAll(new AutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider));
                });

                return new Mapper(mapperConfiguration);
            }
        }
    }
}