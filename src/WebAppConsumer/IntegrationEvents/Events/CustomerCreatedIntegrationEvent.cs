

using EventBus.Events;

namespace WebAppConsumer.IntegrationEvents.Events
{
    public class CustomerCreatedIntegrationEvent : IntegrationEvent
    {
        public string Nome { get; set; }
        public string Email { get; set; }

        public CustomerCreatedIntegrationEvent(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }
    }
}