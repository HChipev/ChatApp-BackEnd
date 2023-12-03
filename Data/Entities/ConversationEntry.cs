using Data.Entities.Abstract;

namespace Data.Entities
{
    public class ConversationEntry : IBaseEntity
    {
        public int Id { get; set; }

        public bool IsFromUser { get; set; }

        public string Text { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}