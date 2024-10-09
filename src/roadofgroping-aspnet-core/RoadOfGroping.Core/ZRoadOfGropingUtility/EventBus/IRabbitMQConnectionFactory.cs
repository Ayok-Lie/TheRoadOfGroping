using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus
{
    public interface IRabbitMQConnectionFactory
    {
        IConnection CreateConnection();

        IModel CreateModel(IConnection connection);
    }

    public class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public RabbitMQConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };
            try
            {
                return factory.CreateConnection();
            }
            catch (Exception ex)
            {
                // 记录日志或抛出自定义异常
                throw new ApplicationException("Failed to connect to RabbitMQ", ex);
            }
        }

        public IModel CreateModel(IConnection connection)
        {
            return connection.CreateModel();
        }
    }
}