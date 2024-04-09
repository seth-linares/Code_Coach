namespace SeniorProjBackend.Data
{
    public class Feedback
    {
        /*
        9.  **Feedback Table**:
    
            *   `FeedbackID` (Primary Key, INT).
            *   `UserID` (INT, Foreign Key, references `Users.UserID`).
            *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`, optional).
            *   `FeedbackText` (VARCHAR(MAX)).
            *   `SubmissionTime` (DateTime).
        */
        public int FeedbackID { get; set; }
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public int? ProblemID { get; set; } // Foreign Key; Problems.ProblemID
        public string FeedbackText { get; set; } // Varchar Max
        public DateTime SubmissionTime { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Problem? Problem { get; set; }
    }
}
