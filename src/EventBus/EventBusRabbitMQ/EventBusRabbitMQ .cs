using System.Net.Sockets;
using System.Text.Json;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private string _queueName;
        private ILogger<EventBusRabbitMQ> _logger;
        private readonly int _retryCount;
        private IModel _consumerChannel;
        const string BROKER_NAME = "event_bus";

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, string queueName, ILogger<EventBusRabbitMQ> logger, int retryCount = 5)
        {
            _persistentConnection = persistentConnection;
            _queueName = queueName;
            _logger = logger;
            _consumerChannel = CreateConsumerChannel();
            _retryCount = retryCount;
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning("Could not publish event: {EventId} after {Timeout}s", @event.Id, time.TotalSeconds);
                });

            var eventName = @event.GetType().Name;

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, inputType: @event.GetType());

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();

                properties.DeliveryMode = 2;

                channel.BasicPublish(exchange: BROKER_NAME,
                                     routingKey: eventName,
                                     mandatory: false,
                                     basicProperties: properties,
                                     body: body);
            });

        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler
        {
            var eventName = typeof(TEvent).Name;

            DoInternalSubscription(eventName);

            StartBasicConsume();
        }

        public void DoInternalSubscription(string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel.QueueBind(queue: _queueName
                                      , exchange: BROKER_NAME
                                      , routingKey: eventName);
        }

        public IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, ExchangeType.Direct);

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (object? sender, CallbackExceptionEventArgs e) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        public void StartBasicConsume()
        {
            if (_consumerChannel is { })
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(queue: _queueName
                                             , autoAck: false
                                             , consumer: consumer);
            }
        }

        public async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            _consumerChannel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }

        public void Dispose()
        {
            if (_consumerChannel is { })
            {
                _consumerChannel.Dispose();
            }
        }
    }
}

