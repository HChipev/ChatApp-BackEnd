using Service.Abstract;

namespace Service.Interfaces
{
    public interface IMessageProducer : IService
    {
        public void SendMessage<T>(T message, string queue);
    }
}