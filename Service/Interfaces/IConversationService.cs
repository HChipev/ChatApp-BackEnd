using Common.Classes;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IConversationService : IService
    {
        public ServiceResult<bool> GenerateAnswer(GenerateQuestionViewModel model);

        public void AddConversation(GenerateAnswerQueue model);

        public ServiceResult<UserConversationViewModel> GetUserConversations(int? userId);
    }
}