using Data.ViewModels.Document.Models;

namespace Data.ViewModels.RabbitMQ.Models
{
    public class LoadDocumentsQueue
    {
        public IEnumerable<LoadDocumentViewModel> Documents { get; set; } = new List<LoadDocumentViewModel>();
    }
}