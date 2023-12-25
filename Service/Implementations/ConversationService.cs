using AutoMapper;
using Common.Classes;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.BasicResponseModels;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Service.Hubs;
using Service.Interfaces;

namespace Service.Implementations
{
    public class ConversationService : IConversationService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Conversation> _conversationRepository;
        private readonly IEventBus _eventBus;
        private readonly IHubContext<RefetchConversationsHub> _hubContext;
        private readonly IMapper _mapper;

        public ConversationService(IEventBus eventBus, IMapper mapper, IRepository<Conversation> conversationRepository,
            IHubContext<RefetchConversationsHub> hubContext, IConfiguration configuration)
        {
            _eventBus = eventBus;
            _mapper = mapper;
            _conversationRepository = conversationRepository;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        public async Task<ServiceResult<ConversationSimpleViewModel>> GenerateAnswerAsync(int userId,
            GenerateQuestionViewModel model)
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
                    conversationSimpleViewModel = await AddNewConversation(_mapper.Map<GenerateAnswerQueue>(model));
                    message.ConversationId = conversationSimpleViewModel.ConversationId;
                }

                if (model.ConversationId is not null)
                {
                    var result =
                        await AddToExistingConversationAsync(userId, _mapper.Map<GenerateAnswerQueue>(model), true);

                    if (!result.IsSuccess)
                    {
                        return new ServiceResult<ConversationSimpleViewModel>
                            { IsSuccess = false, Data = null, Message = result.Message };
                    }

                    conversationSimpleViewModel = result.Data;
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
                    _conversationRepository.FindAllByCondition(x => x.UserId == userId && x.IsDeleted == false));

            userConversation = userConversation.OrderByDescending(x => x.ModifiedAtUtc).ToList();
            return new ServiceResult<UserConversationViewModel>
            {
                IsSuccess = true, Data = new UserConversationViewModel { Conversations = userConversation },
                Message = ""
            };
        }

        public ServiceResult<ConversationsViewModel> GetConversation(int userId, int conversationId)
        {
            var conversation = _conversationRepository.Find(conversationId, x => x.Entries);

            if (conversation is null)
            {
                return new ServiceResult<ConversationsViewModel>
                {
                    IsSuccess = false, Data = null,
                    Message = "No conversation found!"
                };
            }

            if (conversation.IsDeleted)
            {
                return new ServiceResult<ConversationsViewModel>
                {
                    IsSuccess = false, Data = null,
                    Message = "Conversation is deleted!"
                };
            }

            if (userId != conversation.UserId && !conversation.IsShareable)
            {
                return new ServiceResult<ConversationsViewModel>
                {
                    IsSuccess = false, Data = null,
                    Message = "Can't open conversation!"
                };
            }

            var conversationsList = _mapper.Map<List<ConversationViewModel>>(conversation.Entries);

            var mappedConversation = _mapper.Map<ConversationsViewModel>(conversationsList);
            mappedConversation.userId = conversation.UserId;

            return new ServiceResult<ConversationsViewModel>
            {
                IsSuccess = true, Data = mappedConversation,
                Message = ""
            };
        }

        public async Task<ConversationSimpleViewModel> AddNewConversation(GenerateAnswerQueue model)
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

            var addedConversation = await _conversationRepository.AddAsync(conversation);
            await _conversationRepository.SaveChangesAsync();

            return _mapper.Map<ConversationSimpleViewModel>(addedConversation);
        }

        public async Task<ServiceResult<BasicResponseViewModel>> DeleteConversationAsync(int userId, int conversationId)
        {
            var conversationForDeletion = _conversationRepository.Find(conversationId);

            if (conversationForDeletion is null)
            {
                return new ServiceResult<BasicResponseViewModel>
                    { Data = null, IsSuccess = false, Message = "No conversation found!" };
            }

            if (userId != conversationForDeletion.UserId)
            {
                return new ServiceResult<BasicResponseViewModel>
                    { Data = null, IsSuccess = false, Message = "Unauthorized!" };
            }

            _conversationRepository.Delete(conversationId);

            await _conversationRepository.SaveChangesAsync();

            await _hubContext.Clients.Group(userId.ToString()).SendAsync("RefetchConversations");

            return new ServiceResult<BasicResponseViewModel>
            {
                Data = new BasicResponseViewModel { Message = "Successfully deleted conversation." }, IsSuccess = true,
                Message = ""
            };
        }

        public async Task<ServiceResult<ShareConversationViewModel>> ShareConversationAsync(int userId,
            int conversationId)
        {
            var conversationForSharing = _conversationRepository.Find(conversationId);

            if (conversationForSharing is null)
            {
                return new ServiceResult<ShareConversationViewModel>
                    { Data = null, IsSuccess = false, Message = "No conversation found!" };
            }

            if (conversationForSharing.IsShareable)
            {
                return new ServiceResult<ShareConversationViewModel>
                {
                    Data = new ShareConversationViewModel
                    {
                        Link =
                            $"{Environment.GetEnvironmentVariable("FRONT_END_URL") ?? _configuration["FrontEndURL"]}{conversationForSharing.Id}"
                    },
                    IsSuccess = true, Message = ""
                };
            }

            if (userId != conversationForSharing.UserId)
            {
                return new ServiceResult<ShareConversationViewModel>
                    { Data = null, IsSuccess = false, Message = "Unauthorized!" };
            }

            conversationForSharing.IsShareable = true;
            _conversationRepository.Update(conversationForSharing);

            await _conversationRepository.SaveChangesAsync();

            return new ServiceResult<ShareConversationViewModel>
            {
                Data = new ShareConversationViewModel
                {
                    Link =
                        $"{Environment.GetEnvironmentVariable("FRONT_END_URL") ?? _configuration["FrontEndURL"]}{conversationForSharing.Id}"
                },
                IsSuccess = true, Message = ""
            };
        }

        public async Task<ServiceResult<ConversationSimpleViewModel>> AddToExistingConversationAsync(int userId,
            GenerateAnswerQueue model,
            bool isFromUser)
        {
            var conversation = _conversationRepository.Find(model.ConversationId.Value, x => x.Entries);

            if (conversation is null)
            {
                return new ServiceResult<ConversationSimpleViewModel>
                    { Data = null, IsSuccess = false, Message = "No conversation found!" };
            }

            if (userId != conversation.UserId)
            {
                return new ServiceResult<ConversationSimpleViewModel>
                    { Data = null, IsSuccess = false, Message = "Unauthorized!" };
            }

            conversation.ChatHistory = model.ChatHistory;
            conversation.Entries.Add(new ConversationEntry
            {
                Text = isFromUser ? model.Question : model.Answer,
                IsFromUser = isFromUser,
                ConversationId = conversation.Id
            });

            _conversationRepository.Update(conversation);

            await _conversationRepository.SaveChangesAsync();

            await _hubContext.Clients.Group(model.UserId.ToString()).SendAsync("RefetchConversations");

            return new ServiceResult<ConversationSimpleViewModel>
                { Data = _mapper.Map<ConversationSimpleViewModel>(conversation), IsSuccess = true, Message = "" };
        }
    }
}