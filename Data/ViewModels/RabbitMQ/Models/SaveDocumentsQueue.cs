using Data.ViewModels.Document.Models;

namespace Data.ViewModels.RabbitMQ.Models
{
    public class SaveDocumentsQueue
    {
        public IEnumerable<SaveDocumentViewModel> Documents { get; set; } = new List<SaveDocumentViewModel>();
    }
}