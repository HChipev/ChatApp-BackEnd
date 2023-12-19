namespace Data.ViewModels.RabbitMQ.Models
{
    public class GenerateQuestionQueue
    {
        public string Question { get; set; } = "";
        public int UserId { get; set; }
        public int? ConversationId { get; set; }
        public string ChatHistory { get; set; } = "[]";
        public string Sid { get; set; } = "";
    }
}