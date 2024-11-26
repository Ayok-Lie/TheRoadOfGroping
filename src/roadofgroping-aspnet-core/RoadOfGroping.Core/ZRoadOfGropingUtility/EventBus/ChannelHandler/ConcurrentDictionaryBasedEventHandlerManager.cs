using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Attributes;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.ChannelHandler
{
    public class ConcurrentDictionaryBasedEventHandlerManager : IConcurrentDictionaryBasedEventHandlerManager
    {
        private readonly ILogger<IConcurrentDictionaryBasedEventHandlerManager> _logger;
        public ConcurrentBag<EventDiscription> Events { get; private set; }

        private readonly ConcurrentDictionary<string, Channel<string>> _channels;

        private readonly IServiceProvider _serviceProvider;
        private bool _isInitConsumer = true;
        private readonly CancellationToken _cancellation;

        public ConcurrentDictionaryBasedEventHandlerManager(
            ILogger<IConcurrentDictionaryBasedEventHandlerManager> logger,
            IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cancellation = CancellationToken.None;
            Events = new ConcurrentBag<EventDiscription>();
            _channels = new ConcurrentDictionary<string, Channel<string>>();
        }

        /// <summary>
        /// 订阅事件并注入EventHandler
        /// </summary>
        /// <typeparam name="TEto">事件类型</typeparam>
        /// <typeparam name="THandler">事件处理器类型</typeparam>
        public void Subscribe<TEto, THandler>()
            where TEto : class
            where THandler : IEventHandle<TEto>
        {
            Subscribe(typeof(TEto), typeof(THandler));
        }

        /// <summary>
        /// 订阅事件并注入EventHandler
        /// </summary>
        /// <param name="eto">事件类型</param>
        /// <param name="handler">事件处理器类型</param>
        private void Subscribe(Type eto, Type handler)
        {
            if (!CheckEvents(eto))
            {
                return;
            }

            Events.Add(new EventDiscription(eto, handler));
        }

        /// <summary>
        /// 后台执行异步任务，将事件数据写入通道
        /// </summary>
        /// <typeparam name="TEto">事件类型</typeparam>
        /// <param name="eto">事件数据</param>
        /// <returns></returns>
        public async Task BackgroundExecuteAsync<TEto>(TEto eto)
            where TEto : class
        {
            var channel = Check(typeof(TEto));

            while (await channel.Writer.WaitToWriteAsync())
            {
                var data = JsonConvert.SerializeObject(eto);

                await channel.Writer.WriteAsync(data, _cancellation);

                break;
            }
        }

        /// <summary>
        /// 直接执行事件处理器
        /// </summary>
        /// <typeparam name="TEto">事件类型</typeparam>
        /// <param name="eto">事件数据</param>
        /// <returns></returns>
        public async Task ExecuteAsync<TEto>(TEto eto)
            where TEto : class
        {
            Check(typeof(TEto));

            var scope = _serviceProvider.CreateAsyncScope();

            var handler = scope.ServiceProvider.GetRequiredService<IEventHandle<TEto>>();

            await handler.Handle(eto);
        }

        /// <summary>
        /// 启动后台消费者
        /// </summary>
        /// <returns></returns>
        public async Task StartConsumer()
        {
            if (!_isInitConsumer)
            {
                return;
            }

            foreach (var item in Events)
            {
                _ = Task.Factory.StartNew(async () =>
                {
                    var channel = Check(item.EtoType);

                    var handlerType = typeof(IEventHandle<>).MakeGenericType(item.EtoType);
                    var scope = _serviceProvider.CreateAsyncScope();
                    var handler = scope.ServiceProvider.GetRequiredService(handlerType);

                    var reader = channel.Reader;

                    try
                    {
                        while (await channel.Reader.WaitToReadAsync())
                        {
                            while (reader.TryRead(out string str))
                            {
                                var data = JsonConvert.DeserializeObject(str, item.EtoType);

                                _logger.LogInformation(str);

                                await (Task)handlerType
                                    .GetMethod("Handle")
                                    .Invoke(handler, new object[] { data });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"本地事件总线异常{e.Source}--{e.Message}--{e.Data}");
                        throw;
                    }
                }, TaskCreationOptions.LongRunning);
            }
            _isInitConsumer = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 检查并获取通道
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <returns>通道</returns>
        private Channel<string> Check(Type type)
        {
            var attribute = type.GetCustomAttributes().OfType<EventDiscriptorAttribute>().FirstOrDefault();

            if (attribute is null)
            {
                throw new Exception("Eto请实现EventDiscriptorAttribute特性");
            }

            if (!_channels.TryGetValue(attribute.EventName, out var channel))
            {
                channel = Channel.CreateBounded<string>(
                    new BoundedChannelOptions(attribute.Capacity)
                    {
                        SingleWriter = true,
                        SingleReader = attribute.SigleReader,
                        AllowSynchronousContinuations = false, // 异步方式执行
                        FullMode = BoundedChannelFullMode.Wait // 等待存在空间进行写入
                    }
                );

                _channels.TryAdd(attribute.EventName, channel);
                _logger.LogInformation($"创建通信管道{type}--{attribute.EventName}");
            }

            return channel;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _isInitConsumer = true;
            _cancellation.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 检查事件是否已订阅
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <returns>是否已订阅</returns>
        private bool CheckEvents(Type type)
        {
            var description = Events.FirstOrDefault(p => p.EtoType == type);

            return description is null;
        }
    }
}