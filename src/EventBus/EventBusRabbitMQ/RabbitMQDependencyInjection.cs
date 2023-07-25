using EventBus.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBusRabbitMQ
{
    public static class RabbitMQDependencyInjection
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("EventBus").Bind(rabbitMQSettings);


            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory
                {
                    HostName = rabbitMQSettings.HostName,
                    UserName = rabbitMQSettings.UserName,
                    Password = rabbitMQSettings.Password,
                    DispatchConsumersAsync = true,
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger, 5);
            });

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var persistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();

                return new EventBusRabbitMQ(persistentConnection, rabbitMQSettings.QueueName, logger);
            });

            return services;
        }
    }
}