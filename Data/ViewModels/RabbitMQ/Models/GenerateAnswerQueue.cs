namespace Data.ViewModels.RabbitMQ.Models
{
    public class GenerateAnswerQueue
    {
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public int UserId { get; set; }
        public int? ConversationId { get; set; }
        public string ChatHistory { get; set; } = "[]";
    }
}