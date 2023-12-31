using Data.ViewModels.RabbitMQ.Models;
using Service.EventHandlers.Interfaces;
using Service.Interfaces;

namespace Service.EventHandlers.Implementations
{
    public class SaveDocumentsEventHandler : IEventHandler<SaveDocumentsQueue>
    {
        private readonly IDocumentService _documentService;

        public SaveDocumentsEventHandler(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public async Task Handle(SaveDocumentsQueue @event)
        {
            await _documentService.UpdateDocumentsAsync(@event);
        }
    }
}