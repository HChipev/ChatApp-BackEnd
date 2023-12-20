using Common.Classes;
using Data.ViewModels.BasicResponseModels;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IConversationService : IService
    {
        public Task<ServiceResult<ConversationSimpleViewModel>> GenerateAnswerAsync(int userId,
            GenerateQuestionViewModel model);

        public ConversationSimpleViewModel AddNewConversation(GenerateAnswerQueue model);

        public Task<ServiceResult<ConversationSimpleViewModel>> AddToExistingConversationAsync(int userId,
            GenerateAnswerQueue model,
            bool isFromUser);

        public ServiceResult<UserConversationViewModel> GetConversationsByUserId(int userId);

        public ServiceResult<ConversationsViewModel> GetConversation(int userId, int conversationId);

        public Task<ServiceResult<BasicResponseViewModel>> DeleteConversationAsync(int userId, int conversationId);

        public ServiceResult<ShareConversationViewModel> ShareConversation(int userId, int conversationId);
    }
}