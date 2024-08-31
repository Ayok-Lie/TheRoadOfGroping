using System.Collections.Concurrent;

namespace RoadOfGroping.Utility.EventBus
{
    public interface IConcurrentDictionaryBasedEventHandlerManager : IDisposable
    {
        Task BackgroundExecuteAsync<TEto>(TEto eto) where TEto : class;

        Task StartConsumer();

        Task ExecuteAsync<TEto>(TEto eto) where TEto : class;

        public void Subscribe<TEto, THandler>() where TEto : class where THandler : IEventHandle<TEto>;

        public ConcurrentBag<EventDiscription> Events { get; }
    }
}

//public static class EventBusExtensions
//{
//    public static IServiceCollection AddEventBus(this IServiceCollection services)
//    {
//        services.AddSingleton<IEventBus, EventBus>();

//        var serviceProvider = services.BuildServiceProvider();
//        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

//        // 获取所有实现了BaseEventHandler的类型
//        var eventHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
//            .SelectMany(a => a.GetTypes())
//            .Where(t => t.IsSubclassOf(typeof(BaseEventHandler)));

//        foreach (var handlerType in eventHandlerTypes)
//        {
//            // 获取事件类型
//            var eventType = handlerType.BaseType.GetGenericArguments()[0];

//            // 订阅事件
//            var subscribeMethod = typeof(IEventBus).GetMethod("Subscribe")
//                .MakeGenericMethod(eventType, handlerType);
//            subscribeMethod.Invoke(eventBus, new object[] { Activator.CreateInstance(eventType), Activator.CreateInstance(handlerType) });

//            // 注册事件处理器
//            services.AddTransient(handlerType);
//        }

//        return services;
//    }
//}