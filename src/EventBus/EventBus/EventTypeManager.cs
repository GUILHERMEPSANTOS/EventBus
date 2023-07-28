using EventBus.Events;

namespace EventBus
{
    public class EventTypeManager : IEventTypeManager
    {
        private readonly List<Type> _eventTypes;

        public EventTypeManager()
        {
            _eventTypes = new List<Type>();
        }

        public void AddEventType<TEvent>()
        {
            var hasEventType = ContainsEventType<TEvent>();

            if (hasEventType) return;

            var eventType = GetEventType<TEvent>();

            _eventTypes.Add(eventType);
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _eventTypes.SingleOrDefault(eventType => eventType.Name == eventName, typeof(IntegrationEvent));
        }

        private bool ContainsEventType<TEvent>()
        {
            var eventType = GetEventType<TEvent>();

            return _eventTypes.Contains(eventType);
        }

        private Type GetEventType<TEvent>()
        {
            return typeof(TEvent);
        }

    }
}