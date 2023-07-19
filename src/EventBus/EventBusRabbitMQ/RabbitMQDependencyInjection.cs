using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBusRabbitMQ
{
    public static class RabbitMQDependencyInjection
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("EventBus").Bind(rabbitMQSettings);
    
            return services;
        }
    }
}