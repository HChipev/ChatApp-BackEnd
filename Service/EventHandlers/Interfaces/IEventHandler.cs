namespace Service.EventHandlers.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        void Handle(TEvent @event);
    }
}