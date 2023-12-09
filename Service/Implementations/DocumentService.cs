using AutoMapper;
using Common.Classes;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Interfaces;

namespace Service.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public DocumentService(IMapper mapper, IRepository<Document> documentRepository, IEventBus eventBus)
        {
            _mapper = mapper;
            _documentRepository = documentRepository;
            _eventBus = eventBus;
        }

        public ServiceResult<bool> AddDocuments(DocumentsViewModel models)
        {
            try
            {
                var documents = _documentRepository.AddRange(_mapper.Map<List<Document>>(models.Documents));

                _documentRepository.SaveChanges();

                _eventBus.Publish(_mapper.Map<LoadDocumentsQueue>(documents.ToList()));

                return new ServiceResult<bool> { IsSuccess = true, Data = true, Message = "" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = ex.Message };
            }
        }

        public void UpdateDocuments(SaveDocumentsQueue models)
        {
            _documentRepository.UpdateRange(_mapper.Map<List<Document>>(models.Documents));

            _documentRepository.SaveChanges();
        }

        public ServiceResult<DocumentsSimpleViewModel> GetDocuments()
        {
            var documentsSimple =
                _mapper.Map<List<DocumentSimpleViewModel>>(
                    _documentRepository.GetAll());

            return new ServiceResult<DocumentsSimpleViewModel>
            {
                IsSuccess = true, Data = new DocumentsSimpleViewModel { Documents = documentsSimple },
                Message = ""
            };
        }
    }
}