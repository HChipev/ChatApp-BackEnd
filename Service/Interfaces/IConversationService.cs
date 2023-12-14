using Common.Classes;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IConversationService : IService
    {
        public Task<ServiceResult<ConversationSimpleViewModel>> GenerateAnswerAsync(GenerateQuestionViewModel model);

        public ConversationSimpleViewModel AddNewConversation(GenerateAnswerQueue model);

        public Task<ConversationSimpleViewModel> AddToExistingConversationAsync(GenerateAnswerQueue model,
            bool isFromUser);

        public ServiceResult<UserConversationViewModel> GetConversationsByUserId(int userId);

        public ServiceResult<ConversationsViewModel> GetConversation(int userId, int conversationId);
    }
}