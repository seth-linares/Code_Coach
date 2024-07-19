namespace SeniorProjBackend.Data
{
    public class AIMessage
    {
        public int MessageID { get; set; }
        public int ConversationID { get; set; }
        public string Content { get; set; }
        public string Role { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        // Navigation property
        public AIConversation Conversation { get; set; }
    }
}
