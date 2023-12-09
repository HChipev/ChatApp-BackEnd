using Common.Enums;
using Data.Entities.Abstract;

namespace Data.Entities
{
    public class Document : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public DocumentType Type { get; set; }

        public byte[] Bytes { get; set; }

        public IEnumerable<string> VectorIds { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}