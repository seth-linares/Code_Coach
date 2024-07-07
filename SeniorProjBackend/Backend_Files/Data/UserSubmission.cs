namespace SeniorProjBackend.Data
{
    public class UserSubmission
    {

        /*
         * 3.  **UserSubmissions Table**:   
        *   `SubmissionID` (Primary Key, INT).
        *   `UserID` (INT, Foreign Key, references `Users.UserID`).
        *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`).
        *   `SubmittedCode` (VARCHAR(MAX)).
        *   `SubmissionTime` (DateTime).
        *   `IsSuccessful` (Boolean).
        *   `ScoreAwarded` (INT).
        *   `ExecutionTime` (INT, optional): Time taken to execute the solution.
        *   `MemoryUsage` (INT, optional): Memory used by the solution.
        *   `LanguageID` (INT, Foreign Key, references `Languages.LanguageID`): Programming language used in the submission.
        */
        public int SubmissionID { get; set; }
        public int UserId { get; set; } // Foreign Key; Users.UserId
        public int ProblemID { get; set; } // Foreign Key; Problems.ProblemID
        public int LanguageID { get; set; } // Foreign Key; Languages.LanguageID
        public string SubmittedCode { get; set; } // Varchar Max
        public DateTime SubmissionTime { get; set; }
        public bool IsSuccessful { get; set; }
        public int ScoreAwarded { get; set; } // This might belong in the Problems table, not sure
        public int? ExecutionTime { get; set; } // ET and MU might be worth scrapping, we'll see as we go on
        public int? MemoryUsage { get; set; }

        /*
         * If the score awarded is specific to each user submission and can vary based on factors like execution time or memory usage, 
         * then keeping it in the UserSubmission table makes sense. However, if the score is directly associated with the problem itself 
         * and remains constant for all submissions, you could consider moving it to the Problem table instead.
         */ 

        // Navigation properties
        public User User { get; set; }
        public Problem Problem { get; set; }
        public Language Language { get; set; }
    }
}
