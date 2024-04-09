namespace SeniorProjBackend.Data
{
    public class AIConversation
    {
        /*
        10. **AIConversations Table**:

            *   `ConversationID` (Primary Key, INT): Unique identifier for each conversation.
            *   `UserID` (INT, Foreign Key, references `Users.UserID`): Identifier of the user who had the conversation.
            *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`): Identifier of the problem related to the conversation.
            *   `Timestamp` (DateTime): Date and time when the conversation took place.
            *   `ConversationContent` (NVARCHAR(MAX)): The entire conversation text, possibly in a structured format like JSON to preserve the flow of the conversation.
            *   `IsCompleted` (Boolean): Indicates whether the problem was solved in this session or if the conversation is ongoing.
        */
        public int ConversationID { get; set; }
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public int? ProblemID { get; set; } // Foreign Key; Problems.ProblemID
        public DateTime Timestamp { get; set; }
        public string ConversationContent { get; set; } // Varchar max
        public bool IsCompleted { get; set; } 


        // Navigation properties
        public User User { get; set; }
        public Problem? Problem { get; set; }


    }
}
