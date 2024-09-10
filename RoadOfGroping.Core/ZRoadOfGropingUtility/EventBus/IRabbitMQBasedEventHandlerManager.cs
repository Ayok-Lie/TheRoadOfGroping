namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public interface IRabbitMQBasedEventHandlerManager
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="TEvent">表示要订阅的事件类型。它是泛型参数，必须继承自 IntegrationEvent 类</typeparam>
        /// <param name="event">事件</param>
        void Publish<TEvent>(TEvent @event) where TEvent : IntegrationEvent;

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler">表示要处理该事件的事件处理程序类型。它是泛型参数，必须实现 IIntegrationEventHandler<TEvent> 接口</typeparam>
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IntegrationEventHandle<TEvent>;

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IntegrationEventHandle<TEvent>;
    }
}