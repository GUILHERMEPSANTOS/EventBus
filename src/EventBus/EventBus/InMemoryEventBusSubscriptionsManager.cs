using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionManager
    {
        private readonly IEventTypeManager _eventTypeManager;
        private readonly IDictionary<string, List<SubscriptionInfo>> _handlers;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypeManager = new EventTypeManager();
        }

        public void AddSubscription<TEvent, TEventHanlder>()
            where TEvent : IntegrationEvent
            where TEventHanlder : IIntegrationEventHandler
        {
            DoAddSubscription<TEvent, TEventHanlder>();

            _eventTypeManager.AddEventType<TEvent>();
        }

        private void DoAddSubscription<TEvent, TEventHanlder>()
            where TEvent : IntegrationEvent
            where TEventHanlder : IIntegrationEventHandler
        {
            var eventName = GetTypeName<TEvent>();
            var handlerType = GetType<TEventHanlder>();
            var alreadyExists = HasSubscriptionForEvent<TEvent>();

            if (!alreadyExists)
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (HasSubscriptionInfoForEvent(eventName, handlerType))
            {
                throw new ArgumentException(
                     $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
        }

        private string GetTypeName<TEvent>()
        {
            return typeof(TEvent).Name;
        }

        private Type GetType<T>()
        {
            return typeof(T);
        }

        public bool HasSubscriptionForEvent<TEvent>() where TEvent : IntegrationEvent
        {
            var eventKey = GetTypeName<TEvent>();

            return _handlers.ContainsKey(eventKey);
        }

        public bool HasSubscriptionInfoForEvent(string eventName, Type? handlerType)
        {
            return _handlers[eventName].Any(handler => handler.HandlerType == handlerType);
        }
    }
}