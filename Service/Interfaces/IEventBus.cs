using Service.EventHandlers.Interfaces;

namespace Service.Interfaces
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event) where TEvent : class;

        void Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler<TEvent>;
    }
}