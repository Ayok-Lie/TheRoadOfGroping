using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.ChannelHandler;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.RabbitMQHandler;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Extensions
{
    public static class EventBusExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
            services.AddSingleton<IRabbitMQBasedEventHandlerManager, RabbitMQBasedEventHandlerManager>();

            var serviceProvider = services.BuildServiceProvider();
            var eventBusServices = serviceProvider.GetRequiredService<IRabbitMQBasedEventHandlerManager>();

            // 获取所有实现了IIntegrationEventHandler接口的类型
            var eventHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IntegrationEventHandle<>)));

            foreach (var handlerType in eventHandlerTypes)
            {
                // 获取事件类型
                var eventType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IntegrationEventHandle<>))
                    .GetGenericArguments()[0];

                // 订阅事件
                var subscribeMethod = typeof(IRabbitMQBasedEventHandlerManager).GetMethod("Subscribe")
                    .MakeGenericMethod(eventType, handlerType);
                subscribeMethod.Invoke(eventBusServices, null);

                // 注册事件处理器
                services.AddTransient(handlerType);
            }

            return services;
        }

        public static IServiceCollection AddEventBusAndSubscribes(this IServiceCollection services, Action<IConcurrentDictionaryBasedEventHandlerManager> action)
        {
            services.AddSingleton<IConcurrentDictionaryBasedEventHandlerManager, ConcurrentDictionaryBasedEventHandlerManager>();

            var serviceProvider = services.BuildServiceProvider();
            var eventHandlerContainer = serviceProvider.GetRequiredService<IConcurrentDictionaryBasedEventHandlerManager>();

            action.Invoke(eventHandlerContainer);

            // 注册事件处理器
            foreach (var eventDescription in eventHandlerContainer.Events)
            {
                var handlerbaseType = typeof(IEventHandle<>);

                var handlertype = handlerbaseType.MakeGenericType(eventDescription.EtoType);
                if (services.Any(P => P.ServiceType == handlertype))
                {
                    continue;
                }
                services.AddTransient(handlertype, eventDescription.HandlerType);
            }
            services.AddTransient<ILocalEventBus, LocalEventBus>();

            services.AddHostedService<EventBusBackgroundService>();

            return services;
        }

        //public static IServiceCollection AddEventBus(this IServiceCollection services)
        //{
        //    // 注册 IConcurrentDictionaryBasedEventHandlerManager 的单例实例
        //    services.AddSingleton<IConcurrentDictionaryBasedEventHandlerManager>(sp =>
        //    {
        //        var logger = sp.GetRequiredService<ILogger<IConcurrentDictionaryBasedEventHandlerManager>>();
        //        var serviceProvider = sp.GetRequiredService<IServiceProvider>();
        //        var eventHandlerManager = new ConcurrentDictionaryBasedEventHandlerManager(logger, serviceProvider, services);

        //        // 获取所有实现了 IIntegrationEventHandler<> 接口的类型
        //        var eventHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
        //            .SelectMany(a => a.GetTypes())
        //            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)));

        //        foreach (var handlerType in eventHandlerTypes)
        //        {
        //            // 获取事件类型
        //            var eventType = handlerType.GetInterfaces()
        //                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
        //                .GetGenericArguments()[0];

        //            // 订阅事件
        //            var subscribeMethod = typeof(ConcurrentDictionaryBasedEventHandlerManager).GetMethod("Subscribe")
        //                .MakeGenericMethod(eventType, handlerType);
        //            subscribeMethod.Invoke(eventHandlerManager, new object[] { eventType, handlerType });

        //            // 注册事件处理器
        //            services.AddTransient(handlerType);
        //        }

        //        return eventHandlerManager;
        //    });

        //    return services;
        //}
    }
}