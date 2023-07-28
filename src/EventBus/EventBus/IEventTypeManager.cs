namespace EventBus
{
    public interface IEventTypeManager
    {
        void AddEventType<T>();
        Type GetEventTypeByName(string eventType);
    }
}