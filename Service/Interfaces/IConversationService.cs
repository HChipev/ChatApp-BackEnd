using Common.Classes;
using Data.ViewModels.Conversation.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IConversationService : IService
    {
        public ServiceResult<bool> GenerateAnswer(GenerateAnswerViewModel model);
    }
}