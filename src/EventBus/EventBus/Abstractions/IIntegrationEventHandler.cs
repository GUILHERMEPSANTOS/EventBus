using EventBus.Events;

namespace EventBus.Abstractions;
public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler
    where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event);
}

public interface IIntegrationEventHandler
{

}