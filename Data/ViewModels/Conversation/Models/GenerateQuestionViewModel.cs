namespace Data.ViewModels.Conversation.Models
{
    public class GenerateQuestionViewModel
    {
        public string Question { get; set; } = "";
        public int UserId { get; set; }
        public int? ConversationId { get; set; }
        public string Sid { get; set; } = "";
    }
}