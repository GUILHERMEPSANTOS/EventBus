using EventBus.Abstractions;
using EventBus.Events;

namespace WebAppPublisher.IntegrationEvents
{
    public class CustomerIntegrationEventService : ICustomerIntegrationEventService
    {
        private readonly IEventBus _eventBus;

        public CustomerIntegrationEventService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
        {
            _eventBus.Publish(@event);
        }
    }
}