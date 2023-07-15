using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private readonly int _retryCount;
        public bool IsConnected => _connection is { IsOpen: true };
        public bool Disposed;
        private readonly object _lock = new();

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        public bool TryConnect()
        {
            lock (_lock)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning("RabbitMQ Client could not connect after {TimeOut}s", $"{time.TotalSeconds:n1}");
                    });

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += ConnectionShutdown;
                    _connection.CallbackException += CallbackException;
                    _connection.ConnectionUnblocked += ConnectionUnblocked;

                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("Fatal error: RabbitMQ connections could not be created and opened");
                    return false;
                }
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (Disposed) return;

            TryConnect();
        }
        public void CallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (Disposed) return;

            TryConnect();
        }
        public void ConnectionUnblocked(object? sender, EventArgs e)
        {
            if (Disposed) return;

            TryConnect();
        }


        public void Dispose()
        {
            Disposed = true;

            try
            {
                _connection.ConnectionShutdown -= ConnectionShutdown;
                _connection.CallbackException -= CallbackException;
                _connection.ConnectionUnblocked -= ConnectionUnblocked;
                _connection?.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}