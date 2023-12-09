namespace Data.ViewModels.Conversation.Models
{
    public class ConversationEntryViewModel
    {
        public string Text { get; set; } = "";
        public bool IsFromUser { get; set; }
        public bool CurrentMessageLoading { get; set; } = false;
    }
}