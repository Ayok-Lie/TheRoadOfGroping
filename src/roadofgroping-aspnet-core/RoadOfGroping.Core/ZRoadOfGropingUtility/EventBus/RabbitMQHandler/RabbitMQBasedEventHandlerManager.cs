using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.RabbitMQHandler
{
    public class RabbitMQBasedEventHandlerManager : IRabbitMQBasedEventHandlerManager, IDisposable
    {
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private readonly IConfiguration _configuration;
        private readonly bool _isEnabled;

        public RabbitMQBasedEventHandlerManager(IConfiguration configuration, IRabbitMQConnectionFactory connectionFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _isEnabled = bool.Parse(configuration["RabbitMQ:Enable"] ?? "false");

            if (_isEnabled)
            {
                _connection = connectionFactory.CreateConnection();
                _channel = connectionFactory.CreateModel(_connection);
            }
            else
            {
                _connection = null;
                _channel = null;
            }
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IntegrationEvent
        {
            if (!_isEnabled)
            {
                throw new InvalidOperationException("RabbitMQ is not enabled.");
            }

            var eventName = @event.GetType().Name;
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.ExchangeDeclare(exchange: "events", type: ExchangeType.Direct);
            try
            {
                _channel.BasicPublish(exchange: "events",
                                      routingKey: eventName,
                                      basicProperties: null,
                                      body: body);
            }
            catch (Exception ex)
            {
                // 记录日志或抛出自定义异常
                throw new ApplicationException($"Failed to publish event {eventName}", ex);
            }
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IntegrationEventHandle<TEvent>
        {
            if (!_isEnabled)
            {
                throw new InvalidOperationException("RabbitMQ is not enabled.");
            }

            var eventName = typeof(TEvent).Name;
            _channel.ExchangeDeclare(exchange: "events", type: ExchangeType.Direct);
            _channel?.QueueDeclare(queue: eventName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: eventName, exchange: "events", routingKey: eventName);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var @event = JsonSerializer.Deserialize<TEvent>(message);
                    var handler = Activator.CreateInstance<TEventHandler>();
                    if (@event != null)
                    {
                        await handler.Handle(@event);
                    }
                }
                catch (Exception ex)
                {
                    // 记录日志或抛出自定义异常
                    throw new ApplicationException($"Failed to handle event {eventName}", ex);
                }
            };

            _channel.BasicConsume(queue: eventName, autoAck: true, consumer: consumer);
        }

        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IntegrationEventHandle<TEvent>
        {
            if (!_isEnabled)
            {
                throw new InvalidOperationException("RabbitMQ is not enabled.");
            }

            var eventName = typeof(TEvent).Name;
            _channel.QueueUnbind(queue: eventName, exchange: "events", routingKey: eventName);
        }

        public void Dispose()
        {
            if (_isEnabled)
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
        }
    }
}