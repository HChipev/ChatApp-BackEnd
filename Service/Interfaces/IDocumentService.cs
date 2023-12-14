using Common.Classes;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IDocumentService : IService
    {
        public Task<ServiceResult<bool>> AddDocumentsAsync(DocumentsViewModel models, int userId);
        public void UpdateDocuments(SaveDocumentsQueue models);
        public ServiceResult<DocumentsSimpleViewModel> GetDocuments();
        public Task<ServiceResult<bool>> RestoreDocumentAsync(int documentId, int userId);
        public Task<ServiceResult<bool>> DeleteDocumentAsync(int documentId, int userId);
    }
}