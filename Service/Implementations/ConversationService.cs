using Common.Classes;
using Data.ViewModels.Conversation.Models;
using Service.Interfaces;

namespace Service.Implementations
{
    public class ConversationService : IConversationService
    {
        private readonly IMessageProducer _messageProducer;

        public ConversationService(IMessageProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public ServiceResult<bool> GenerateAnswer(GenerateAnswerViewModel model)
        {
            _messageProducer.SendMessage(model, "generate_question");

            return new ServiceResult<bool> { IsSuccess = true, Data = true, Message = "" };
        }
    }
}