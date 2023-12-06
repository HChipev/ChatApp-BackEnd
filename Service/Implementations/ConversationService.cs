using AutoMapper;
using Common.Classes;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Interfaces;

namespace Service.Implementations
{
    public class ConversationService : IConversationService
    {
        private readonly IRepository<Conversation> _conversationRepository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public ConversationService(IEventBus eventBus, IMapper mapper, IRepository<Conversation> conversationRepository)
        {
            _eventBus = eventBus;
            _mapper = mapper;
            _conversationRepository = conversationRepository;
        }

        public ServiceResult<bool> GenerateAnswer(GenerateQuestionViewModel model)
        {
            try
            {
                var message = _mapper.Map<GenerateQuestionQueue>(model);
                message.ChatHistory = "[]";
                if (model.ConversationId is not null)
                {
                    var conversation = _conversationRepository.Find(model.ConversationId.Value);
                    message.ChatHistory = conversation.ChatHistory;
                }

                _eventBus.Publish(message);

                return new ServiceResult<bool> { IsSuccess = true, Data = true, Message = "" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = ex.Message };
            }
        }

        public void AddConversation(GenerateAnswerQueue model)
        {
            if (model.ConversationId is null)
            {
                var conversation = _mapper.Map<Conversation>(model);

                conversation.Entries = new List<ConversationEntry>
                {
                    new()
                    {
                        Text = model.Question,
                        IsFromUser = true,
                        ConversationId = conversation.Id
                    },
                    new()
                    {
                        Text = model.Answer,
                        IsFromUser = false,
                        ConversationId = conversation.Id
                    }
                };


                _conversationRepository.Add(conversation);
            }

            if (model.ConversationId is not null)
            {
                var conversation = _conversationRepository.Find(model.ConversationId.Value, x => x.Entries);

                conversation?.Entries.Add(new ConversationEntry
                {
                    Text = model.Question,
                    IsFromUser = true,
                    ConversationId = conversation.Id
                });
                conversation?.Entries.Add(new ConversationEntry
                {
                    Text = model.Answer,
                    IsFromUser = false,
                    ConversationId = conversation.Id
                });
            }

            _conversationRepository.SaveChanges();
        }

        public ServiceResult<UserConversationViewModel> GetUserConversations(int? userId)
        {
            if (userId is null)
            {
                return new ServiceResult<UserConversationViewModel>
                    { IsSuccess = false, Data = null, Message = "No user found!" };
            }

            var userConversation =
                _mapper.Map<List<ConversationSimpleViewModel>>(
                    _conversationRepository.FindAllByCondition(x => x.UserId == userId));

            return new ServiceResult<UserConversationViewModel>
            {
                IsSuccess = true, Data = new UserConversationViewModel { Conversations = userConversation },
                Message = ""
            };
        }
    }
}