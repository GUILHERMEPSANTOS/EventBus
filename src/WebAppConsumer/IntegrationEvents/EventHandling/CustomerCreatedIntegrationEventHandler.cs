using EventBus.Abstractions;
using WebAppConsumer.IntegrationEvents.Events;

namespace WebAppConsumer.IntegrationEvents.EventHandling
{
    public class CustomerCreatedIntegrationEventHandler : IIntegrationEventHandler<CustomerCreatedIntegrationEvent>
    {
        public Task Handle(CustomerCreatedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}