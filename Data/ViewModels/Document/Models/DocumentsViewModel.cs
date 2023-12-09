namespace Data.ViewModels.Document.Models
{
    public class DocumentsViewModel
    {
        public IEnumerable<DocumentViewModel> Documents { get; set; } = new List<DocumentViewModel>();
    }
}