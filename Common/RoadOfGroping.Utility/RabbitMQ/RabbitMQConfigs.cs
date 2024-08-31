using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RoadOfGroping.Utility.RabbitMQ
{
    public abstract class RabbitMQConfigs
    {
        protected ConnectionFactory GreateConnectionFactory(IConfiguration configuration)
        {
            // 在这里设置ConnectionFactory的属性
            var factory = new ConnectionFactory
            {
                // 设置连接属性
                HostName = configuration["RabbitMQ:HostName"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            return factory;
        }
    }
}