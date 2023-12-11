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

        public ServiceResult<ConversationSimpleViewModel> GenerateAnswer(GenerateQuestionViewModel model)
        {
            try
            {
                var conversationSimpleViewModel = new ConversationSimpleViewModel();

                var message = _mapper.Map<GenerateQuestionQueue>(model);
                message.ChatHistory = "[]";

                if (model.ConversationId is not null)
                {
                    var conversation = _conversationRepository.Find(model.ConversationId.Value);
                    message.ChatHistory = conversation.ChatHistory;
                }

                if (model.ConversationId is null)
                {
                    conversationSimpleViewModel = AddNewConversation(_mapper.Map<GenerateAnswerQueue>(model));
                    message.ConversationId = conversationSimpleViewModel.ConversationId;
                }

                if (model.ConversationId is not null)
                {
                    conversationSimpleViewModel =
                        AddToExistingConversation(_mapper.Map<GenerateAnswerQueue>(model), true);
                }

                _eventBus.Publish(message);

                return new ServiceResult<ConversationSimpleViewModel>
                    { IsSuccess = true, Data = conversationSimpleViewModel, Message = "" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ConversationSimpleViewModel>
                    { IsSuccess = false, Data = null, Message = ex.Message };
            }
        }

        public ServiceResult<UserConversationViewModel> GetConversationsByUserId(int userId)
        {
            var userConversation =
                _mapper.Map<List<ConversationSimpleViewModel>>(
                    _conversationRepository.FindAllByCondition(x => x.UserId == userId));

            userConversation = userConversation.OrderByDescending(x => x.ModifiedAtUtc).ToList();
            return new ServiceResult<UserConversationViewModel>
            {
                IsSuccess = true, Data = new UserConversationViewModel { Conversations = userConversation },
                Message = ""
            };
        }

        public ServiceResult<List<ConversationViewModel>> GetConversation(int userId, int conversationId)
        {
            var conversation = _conversationRepository.Find(conversationId, x => x.Entries);

            if (conversation is null)
            {
                return new ServiceResult<List<ConversationViewModel>>
                {
                    IsSuccess = false, Data = null,
                    Message = "No conversation found!"
                };
            }

            if (userId != conversation.UserId && !conversation.IsShareable)
            {
                return new ServiceResult<List<ConversationViewModel>>
                {
                    IsSuccess = false, Data = null,
                    Message = "Can't open conversation!"
                };
            }

            var conversationViewModel = _mapper.Map<List<ConversationViewModel>>(conversation.Entries);


            return new ServiceResult<List<ConversationViewModel>>
            {
                IsSuccess = true, Data = conversationViewModel,
                Message = ""
            };
        }

        public ConversationSimpleViewModel AddNewConversation(GenerateAnswerQueue model)
        {
            var conversation = _mapper.Map<Conversation>(model);
            conversation.Entries = new List<ConversationEntry>
            {
                new()
                {
                    Text = model.Question,
                    IsFromUser = true,
                    ConversationId = conversation.Id
                }
            };

            var addedConversation = _conversationRepository.Add(conversation);
            _conversationRepository.SaveChanges();

            return _mapper.Map<ConversationSimpleViewModel>(addedConversation);
        }

        public ConversationSimpleViewModel AddToExistingConversation(GenerateAnswerQueue model, bool isFromUser)
        {
            var conversation = _conversationRepository.Find(model.ConversationId.Value, x => x.Entries);
            if (conversation is not null)
            {
                conversation.ChatHistory = model.ChatHistory;
                conversation.Entries.Add(new ConversationEntry
                {
                    Text = isFromUser ? model.Question : model.Answer,
                    IsFromUser = isFromUser,
                    ConversationId = conversation.Id
                });
            }

            _conversationRepository.SaveChanges();

            return _mapper.Map<ConversationSimpleViewModel>(conversation);
        }
    }
}