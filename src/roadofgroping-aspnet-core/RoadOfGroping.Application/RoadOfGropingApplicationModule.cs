using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Application.Service.Handler;
using RoadOfGroping.Application.Service.Mappers;
using RoadOfGroping.Core;
using RoadOfGroping.Core.Files;
using RoadOfGroping.Core.Files.Dtos;
using RoadOfGroping.Core.ZRoadOfGropingUtility.AutoMapper;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Extensions;
using RoadOfGroping.Model;
using RoadOfGroping.Model.Modules;

namespace RoadOfGroping.Application
{
    [DependOn(typeof(RoadOfGropingCoreModule))]
    public class RoadOfGropingApplicationModule : BaseModule
    {
        /// <summary>
        /// 服务注入
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigerService(ServiceConfigerContext context)
        {
            context.Services.AddSingleton(MapperHepler.CreateMappings);
            context.Services.AddSingleton<IMapper>(provider => provider.GetRequiredService<Mapper>());

            // 注册EventBus服务
            context.Services.AddEventBusAndSubscribes(c =>
            {
                c.Subscribe<TestDto, TestEventHandler>();
                c.Subscribe<FileEventDto, FileEventHandler>();
            });
            ConfigureAutoMapper();
        }

        private void ConfigureAutoMapper()
        {
            Configure<AutoMapperOptions>(options =>
            {
                options.Configurators.Add(ctx =>
                {
                    AutoMappers.CreateMappings(ctx.MapperConfiguration);
                });
            });
        }
    }
}