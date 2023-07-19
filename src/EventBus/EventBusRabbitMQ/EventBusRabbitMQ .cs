using System.Text;
using System.Text.Json;
using EventBus.Abstractions;
using EventBus.Events;
using RabbitMQ.Client;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private string _queueName;
        const string BROKER_NAME = "event_bus";

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, string queueName)
        {
            _persistentConnection = persistentConnection;
            _queueName = queueName;
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var eventName = @event.GetType().Name;

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, inputType: @event.GetType());

            var properties = channel.CreateBasicProperties();

            properties.DeliveryMode = 2;

            channel.BasicPublish(exchange: BROKER_NAME,
                                 routingKey: eventName,
                                 mandatory: false,
                                 basicProperties: properties,
                                 body: body);
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}

// Bind 
// _queueName
// exchangeName
