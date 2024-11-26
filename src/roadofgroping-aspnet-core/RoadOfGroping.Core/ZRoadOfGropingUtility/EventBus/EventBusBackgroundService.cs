using Microsoft.Extensions.Hosting;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.ChannelHandler;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public class EventBusBackgroundService : BackgroundService
    {
        private readonly IConcurrentDictionaryBasedEventHandlerManager _eventHandlerManager;

        public EventBusBackgroundService(IConcurrentDictionaryBasedEventHandlerManager eventHandlerManager)
        {
            _eventHandlerManager = eventHandlerManager;
        }

        /// <summary>
        /// 执行队列中得任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventHandlerManager.StartConsumer();
        }
    }
}