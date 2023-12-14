using AutoMapper;
using Common.Classes;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;
using Microsoft.AspNetCore.SignalR;
using Service.Hubs;
using Service.Interfaces;

namespace Service.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly IEventBus _eventBus;
        private readonly IHubContext<RefetchDocumentsHub> _hubContext;
        private readonly IMapper _mapper;

        public DocumentService(IMapper mapper, IRepository<Document> documentRepository, IEventBus eventBus,
            IHubContext<RefetchDocumentsHub> hubContext)
        {
            _mapper = mapper;
            _documentRepository = documentRepository;
            _eventBus = eventBus;
            _hubContext = hubContext;
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

        public async Task<ServiceResult<bool>> RestoreDocumentAsync(int documentId, int userId)
        {
            try
            {
                var deletedDocument = _documentRepository.Find(documentId);
                if (deletedDocument is null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "No document found!" };
                }

                deletedDocument.IsDeleted = false;

                var restoredDocument = _documentRepository.Update(deletedDocument);
                _documentRepository.SaveChanges();

                _eventBus.Publish(_mapper.Map<LoadDocumentsQueue>(new List<Document> { restoredDocument }));

                await _hubContext.Clients.Group(userId.ToString()).SendAsync("RefetchDocuments");

                return new ServiceResult<bool> { IsSuccess = true, Data = true, Message = "" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<bool>> DeleteDocumentAsync(int documentId, int userId)
        {
            try
            {
                var document = _documentRepository.Find(documentId);
                if (document is null)
                {
                    return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "No document found!" };
                }

                _eventBus.Publish(_mapper.Map<DeleteDocumentsQueue>(new List<Document> { document }));

                document.IsDeleted = true;
                document.VectorIds = new List<string>();
                _documentRepository.Update(document);

                _documentRepository.SaveChanges();

                await _hubContext.Clients.Group(userId.ToString()).SendAsync("RefetchDocuments");

                return new ServiceResult<bool> { IsSuccess = true, Data = true, Message = "" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<bool>> AddDocumentsAsync(DocumentsViewModel models, int userId)
        {
            try
            {
                var documents = _documentRepository.AddRange(_mapper.Map<List<Document>>(models.Documents));

                _documentRepository.SaveChanges();

                _eventBus.Publish(_mapper.Map<LoadDocumentsQueue>(documents.ToList()));

                await _hubContext.Clients.Group(userId.ToString()).SendAsync("RefetchDocuments");

                return new ServiceResult<bool> { IsSuccess = true, Data = true, Message = "" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool> { IsSuccess = false, Data = false, Message = ex.Message };
            }
        }
    }
}