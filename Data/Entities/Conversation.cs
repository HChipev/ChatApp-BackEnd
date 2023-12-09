using Data.Entities.Abstract;

namespace Data.Entities
{
    public class Conversation : IBaseEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ChatHistory { get; set; } = "[]";

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<ConversationEntry> Entries { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsShareable { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}