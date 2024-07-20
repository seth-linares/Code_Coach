namespace SeniorProjBackend.Data
{
    public class AIConversation
    {
        public int ConversationID { get; set; }
        public int UserId { get; set; }
        public int? ProblemID { get; set; }
        public int? LanguageID { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string Model { get; set; } = "gpt-4o-mini";
        public int TotalTokens { get; set; }
        public DateTimeOffset? EndTime { get; set; } // Add this to track when a conversation ends

        // Navigation properties
        public User User { get; set; }
        public Problem? Problem { get; set; }
        public Language? Language { get; set; }
        public List<AIMessage> Messages { get; set; }
    }
}
