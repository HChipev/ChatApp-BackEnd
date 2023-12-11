using Common.Classes;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IDocumentService : IService
    {
        public ServiceResult<bool> AddDocuments(DocumentsViewModel models);
        public void UpdateDocuments(SaveDocumentsQueue models);
        public ServiceResult<DocumentsSimpleViewModel> GetDocuments();
        public ServiceResult<bool> RestoreDocument(int documentId);
        public ServiceResult<bool> DeleteDocument(int documentId);
    }
}