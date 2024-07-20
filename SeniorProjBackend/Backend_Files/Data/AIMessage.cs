namespace SeniorProjBackend.Data
{
    public class AIMessage
    {
        public int MessageID { get; set; }
        public int ConversationID { get; set; }
        public string Content { get; set; }
        public string Role { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }

        // Navigation property
        public virtual AIConversation Conversation { get; set; }
    }
}
