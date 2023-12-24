using Service.EventHandlers.Interfaces;

namespace Service.Interfaces
{
    public interface IEventBus
    {
        public void Publish<TEvent>(TEvent @event) where TEvent : class;

        public void Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler<TEvent>;
    }
}