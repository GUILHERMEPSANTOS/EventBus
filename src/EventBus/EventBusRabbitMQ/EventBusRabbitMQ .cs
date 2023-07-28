using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "event_bus";
        private static readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };
        private static readonly JsonSerializerOptions s_caseInsensitiveOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IServiceProvider _serviceProvider;
        private ILogger<EventBusRabbitMQ> _logger;
        private string _queueName;
        private readonly int _retryCount;
        private IModel _consumerChannel;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection
                               , string queueName
                               , ILogger<EventBusRabbitMQ> logger
                               , IEventBusSubscriptionManager subscriptionManager
                               , IServiceProvider serviceProvider
                               , int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionManager = subscriptionManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _retryCount = retryCount;
            _consumerChannel = CreateConsumerChannel();
            _serviceProvider = serviceProvider;
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
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            DoInternalSubscription(eventName);

            _subscriptionManager.AddSubscription<TEvent, TEventHandler>();

            StartBasicConsume();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subscriptionManager.HasSubscriptionForEvent(eventName);

            if (containsKey) return;

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel.QueueBind(queue: _queueName
                                      , exchange: BROKER_NAME
                                      , routingKey: eventName);
        }

        private IModel CreateConsumerChannel()
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

        private void StartBasicConsume()
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

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            await ProcessEvent(eventName, message);

            _consumerChannel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subscriptionManager.HasSubscriptionForEvent(eventName))
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var subscriptions = _subscriptionManager.GetHandlerForEvent(eventName);

                foreach (var subscription in subscriptions)
                {
                    var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    var handler = scope.ServiceProvider.GetService(concreteType);

                    if (handler is null) continue;

                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, s_caseInsensitiveOptions);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }

        public void Dispose()
        {
            if (_consumerChannel is { })
            {
                _consumerChannel.Dispose();
            }

            _subscriptionManager.Clear();
        }
    }
}

