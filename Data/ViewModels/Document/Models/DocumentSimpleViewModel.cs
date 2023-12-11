namespace Data.ViewModels.Document.Models
{
    public class DocumentSimpleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public byte[] Bytes { get; set; }
        public bool IsDeleted { get; set; }
    }
}