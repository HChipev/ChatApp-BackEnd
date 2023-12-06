using AutoMapper;
using Data.ViewModels.Conversation.Models;
using Data.ViewModels.RabbitMQ.Models;

namespace Data.ViewModels.Conversation.Profiles
{
    public class GenerateQuestionProfile : Profile
    {
        public GenerateQuestionProfile()
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
        }
    }
}