using AutoMapper;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;

namespace Data.ViewModels.Document.Profiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<DocumentViewModel, Entities.Document>();
            CreateMap<Entities.Document, DocumentSimpleViewModel>().ReverseMap();
            CreateMap<Entities.Document, LoadDocumentViewModel>();
            CreateMap<List<Entities.Document>, LoadDocumentsQueue>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src));
            CreateMap<List<Entities.Document>, DeleteDocumentsQueue>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src));
            CreateMap<Entities.Document, DeleteDocumentViewModel>();
            CreateMap<SaveDocumentViewModel, Entities.Document>();
        }
    }
}