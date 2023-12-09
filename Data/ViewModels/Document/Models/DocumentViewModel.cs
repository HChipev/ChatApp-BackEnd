using Common.Enums;

namespace Data.ViewModels.Document.Models
{
    public class DocumentViewModel
    {
        public string Name { get; set; } = "";

        public DocumentType Type { get; set; }

        public byte[] Bytes { get; set; }
    }
}