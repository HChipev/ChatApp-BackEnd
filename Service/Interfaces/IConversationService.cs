using Common.Classes;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IConversationService : IService
    {
        public ServiceResult<ConversationSimpleViewModel> GenerateAnswer(GenerateQuestionViewModel model);

        public ConversationSimpleViewModel AddNewConversation(GenerateAnswerQueue model);

        public ConversationSimpleViewModel AddToExistingConversation(GenerateAnswerQueue model, bool isFromUser);

        public ServiceResult<UserConversationViewModel> GetConversations(int userId);

        public ServiceResult<List<ConversationViewModel>> GetConversation(int userId, int conversationId);
    }
}