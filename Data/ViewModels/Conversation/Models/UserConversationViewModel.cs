namespace Data.ViewModels.Conversation.Models
{
    public class UserConversationViewModel
    {
        public IEnumerable<ConversationSimpleViewModel> Conversations { get; set; } =
            new List<ConversationSimpleViewModel>();
    }
}