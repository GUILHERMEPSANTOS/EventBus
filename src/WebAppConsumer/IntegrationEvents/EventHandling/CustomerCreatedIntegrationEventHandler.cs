using EventBus.Abstractions;
using WebAppConsumer.IntegrationEvents.Events;

namespace WebAppConsumer.IntegrationEvents.EventHandling
{
    public class CustomerCreatedIntegrationEventHandler : IIntegrationEventHandler<CustomerCreatedIntegrationEvent>
    {
        public CustomerCreatedIntegrationEventHandler()
        {

        }
        public Task Handle(CustomerCreatedIntegrationEvent @event)
        {
            Console.WriteLine($"Olá eu sou o {@event.Id}, seu novo email é {@event.Email}");

            return Task.CompletedTask;
        }
    }
}