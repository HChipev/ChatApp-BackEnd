using Data.ViewModels.Document.Models;

namespace Data.ViewModels.RabbitMQ.Models
{
    public class DeleteDocumentsQueue
    {
        public IEnumerable<DeleteDocumentViewModel> Documents { get; set; } = new List<DeleteDocumentViewModel>();
    }
}