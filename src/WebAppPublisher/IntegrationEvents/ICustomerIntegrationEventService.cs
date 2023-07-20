using EventBus.Events;

namespace WebAppPublisher.IntegrationEvents
{
    public interface ICustomerIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent @event);        
    }
}