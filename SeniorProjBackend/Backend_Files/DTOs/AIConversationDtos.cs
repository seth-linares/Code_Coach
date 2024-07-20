namespace SeniorProjBackend.DTOs
{
    public class StartConversationRequest
    {
        public int? ProblemId { get; set; }
        public int? LanguageId { get; set; }
    }

    public class ChatGPTRequest
    {
        public int? ConversationId { get; set; }
        public int ProblemId { get; set; }
        public string Message { get; set; }
    }

    public class MessageDto
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int Tokens { get; set; }
    }

    public class ConversationHistoryDto
    {
        public int ConversationID { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public int? ProblemID { get; set; }
        public int? LanguageID { get; set; }
        public List<MessageDto> Messages { get; set; }
        public int TotalTokens { get; set; }
    }

    public class ConversationListItemDto
    {
        public int ConversationID { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public int? ProblemID { get; set; }
        public int? LanguageID { get; set; }
        public int MessageCount { get; set; }
        public int TotalTokens { get; set; }
    }

    public class ConversationsByProblemRequest
    {
        public int ProblemId { get; set; }
    }
}
