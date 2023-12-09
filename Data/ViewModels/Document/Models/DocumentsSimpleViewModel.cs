namespace Data.ViewModels.Document.Models
{
    public class DocumentsSimpleViewModel
    {
        public IEnumerable<DocumentSimpleViewModel> Documents { get; set; } = new List<DocumentSimpleViewModel>();
    }
}