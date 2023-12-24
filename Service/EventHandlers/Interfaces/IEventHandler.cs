namespace Service.EventHandlers.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        public Task Handle(TEvent @event);
    }
}