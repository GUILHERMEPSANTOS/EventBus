using RabbitMQ.Client;

namespace EventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool TryConnect();
        bool IsConnected { get; }
        IModel CreateModel();
    }
}