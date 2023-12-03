using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Service.Interfaces;

namespace Service.Implementations
{
    public class MessageProducer : IMessageProducer
    {
        private readonly IConfiguration _configuration;

        public MessageProducer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMessage<T>(T message, string queue)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"],
                VirtualHost = _configuration["RabbitMQ:VirtualHost"]
            };

            var connection = factory.CreateConnection();

            using var channel = connection.CreateChannel();
            channel.QueueDeclare(queue, true, false);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish("", queue, body);
        }
    }
}