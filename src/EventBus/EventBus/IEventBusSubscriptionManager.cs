using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus
{
    public interface IEventBusSubscriptionManager
    {
        void AddSubscription<TEvent, TEventHanlder>() 
            where TEvent : IntegrationEvent
            where TEventHanlder : IIntegrationEventHandler;
      
        bool HasSubscriptionForEvent<TEvent>() where TEvent : IntegrationEvent;
        bool HasSubscriptionForEvent(string eventName);
        IEnumerable<SubscriptionInfo> GetHandlerForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
    }
}