namespace SeniorProjBackend.Data
{
    public class AIConversation
    {
        public int ConversationID { get; set; }
        public int UserId { get; set; }
        public int? ProblemID { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string Model { get; set; } = "gpt-4o-mini";
        public int TotalTokens { get; set; }
        public DateTimeOffset? EndTime { get; set; } // Add this to track when a conversation ends

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Problem? Problem { get; set; }
        public virtual List<AIMessage> Messages { get; set; }
    }
}
