namespace Data.ViewModels.Document.Models
{
    public class SaveDocumentViewModel : DocumentViewModel
    {
        public int Id { get; set; }
        public IEnumerable<string> VectorIds { get; set; } = new List<string>();
    }
}