using AutoMapper;
using Data.Entities;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;

namespace Data.ViewModels.Conversation.Profiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<GenerateQuestionViewModel, GenerateQuestionQueue>();

            CreateMap<GenerateAnswerQueue, Entities.Conversation>()
                .ForMember(dest => dest.Title
                    , opt => opt.MapFrom(src => src.Question))
                .ForMember(dest => dest.Id
                    , opt => opt.MapFrom(src => src.ConversationId))
                .ForMember(dest => dest.Entries, opt => opt.Ignore());

            CreateMap<Entities.Conversation, ConversationSimpleViewModel>()
                .ForMember(dest => dest.ConversationId
                    , opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModifiedAtUtc
                    , opt => opt.MapFrom(src => src.ModifiedAt ?? src.CreatedAt));

            CreateMap<ConversationEntry, ConversationViewModel>()
                .ForMember(dest => dest.Message
                    , opt => opt.MapFrom(src => new ConversationEntryViewModel
                    {
                        Text = src.Text,
                        IsFromUser = src.IsFromUser
                    })
                );

            CreateMap<List<ConversationViewModel>, ConversationsViewModel>().ForMember(dest => dest.Messages
                , opt => opt.MapFrom(src => src));

            CreateMap<GenerateQuestionViewModel, GenerateAnswerQueue>();
        }
    }
}