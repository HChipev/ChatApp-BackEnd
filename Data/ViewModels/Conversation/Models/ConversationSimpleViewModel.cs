namespace Data.ViewModels.Conversation.Models
{
    public class ConversationSimpleViewModel
    {
        public int ConversationId { get; set; }
        public string Title { get; set; } = "";
        public DateTime ModifiedAtUtc { get; set; }
    }
}