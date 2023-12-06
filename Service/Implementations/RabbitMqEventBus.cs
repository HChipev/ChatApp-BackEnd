using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.EventHandlers.Interfaces;
using Service.Interfaces;

namespace Service.Implementations
{
    public class RabbitMqEventBus : IEventBus
    {
        private readonly IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqEventBus(string connectionString, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateChannel();
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : class
        {
            var eventName = typeof(TEvent).Name;

            _channel.QueueDeclare(eventName, true, false, true, null);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish("", eventName, body);
        }

        public void Subscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;

            _channel.QueueDeclare(eventName, true, false, true, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonConvert.DeserializeObject<TEvent>(message);

                using var scope = _serviceScopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetService<IEventHandler<TEvent>>();
                handler.Handle(@event);
            };

            _channel.BasicConsume(eventName, true, consumer);
        }
    }
}