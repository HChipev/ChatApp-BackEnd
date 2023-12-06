using Data.ViewModels.RabbitMQ.Models;
using Service.EventHandlers.Interfaces;
using Service.Interfaces;

namespace Service.EventHandlers.Implementations
{
    public class GenerateAnswerEventHandler : IEventHandler<GenerateAnswerQueue>
    {
        private readonly IConversationService _conversationService;

        public GenerateAnswerEventHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public void Handle(GenerateAnswerQueue @event)
        {
            _conversationService.AddConversation(@event);
        }
    }
}