namespace Data.ViewModels.Conversation.Models
{
    public class ConversationsViewModel
    {
        public IEnumerable<ConversationViewModel> Messages { get; set; } = new List<ConversationViewModel>();
    }
}