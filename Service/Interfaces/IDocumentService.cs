using Common.Classes;
using Data.ViewModels.BasicResponseModels;
using Data.ViewModels.Document.Models;
using Data.ViewModels.RabbitMQ.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IDocumentService : IService
    {
        public Task<ServiceResult<BasicResponseViewModel>> AddDocumentsAsync(DocumentsViewModel models, int userId);
        public Task UpdateDocumentsAsync(SaveDocumentsQueue models);
        public ServiceResult<DocumentsSimpleViewModel> GetDocuments();
        public Task<ServiceResult<BasicResponseViewModel>> RestoreDocumentAsync(int documentId, int userId);
        public Task<ServiceResult<BasicResponseViewModel>> DeleteDocumentAsync(int documentId, int userId);
    }
}