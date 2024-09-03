using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RoadOfGroping.Utility.EventBus
{
    public class RabbitMQBasedEventHandlerManager : IRabbitMQBasedEventHandlerManager
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConfiguration _configuration;

        public RabbitMQBasedEventHandlerManager(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory
            {
                // 设置连接属性
                HostName = configuration["RabbitMQ:HostName"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IntegrationEvent
        {
            var eventName = @event.GetType().Name;
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.ExchangeDeclare(exchange: "events", type: ExchangeType.Direct);
            //var properties = _channel.CreateBasicProperties();
            //properties.Headers = new Dictionary<string, object>
            //{
            //    { "x-delay", 5000 } // 延迟 5000 毫秒（5 秒）
            //};
            _channel.BasicPublish(exchange: "events",
                                  routingKey: eventName,
                                  basicProperties: null,
                                  body: body);
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IntegrationEventHandle<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            _channel.ExchangeDeclare(exchange: "events", type: ExchangeType.Direct);
            _channel.QueueDeclare(queue: eventName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: eventName, exchange: "events", routingKey: eventName);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var @event = JsonSerializer.Deserialize<TEvent>(message);
                var handler = Activator.CreateInstance<TEventHandler>();
                if (@event != null)
                {
                    await handler.Handle(@event);
                }
            };

            _channel.BasicConsume(queue: eventName, autoAck: true, consumer: consumer);
        }

        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IntegrationEventHandle<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            _channel.QueueUnbind(queue: eventName, exchange: "events", routingKey: eventName);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}