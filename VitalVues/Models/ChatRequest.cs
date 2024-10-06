namespace VitalVues.Models
{
    public class ChatRequest
    {
        public List<Message>? Messages { get; set; }
    }
    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
