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

            if(hasEventType) return;

            var eventType = GetEventType<TEvent>();

            _eventTypes.Add(eventType);
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