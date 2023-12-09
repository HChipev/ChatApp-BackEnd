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
            CreateMap<Entities.Document, DocumentSimpleViewModel>();
            CreateMap<Entities.Document, LoadDocumentViewModel>();
            CreateMap<List<Entities.Document>, LoadDocumentsQueue>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src));
            CreateMap<SaveDocumentViewModel, Entities.Document>();
        }
    }
}