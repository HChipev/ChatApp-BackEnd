using Common.Classes;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IDocumentService : IService
    {
        public Task<ServiceResult<bool>> AddDocuments(DocumentsViewModel models, int userId);
        public void UpdateDocuments(SaveDocumentsQueue models);
        public ServiceResult<DocumentsSimpleViewModel> GetDocuments();
        public Task<ServiceResult<bool>> RestoreDocument(int documentId, int userId);
        public Task<ServiceResult<bool>> DeleteDocument(int documentId, int userId);
    }
}