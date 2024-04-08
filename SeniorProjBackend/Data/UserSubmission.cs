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
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public int ProblemID { get; set; } // Foreign Key; Problems.ProblemID
        public int LanguageID { get; set; } // Foreign Key; Languages.LanguageID
        public string SubmittedCode { get; set; } // Varchar Max
        public DateTime SubmissionTime { get; set; }
        public bool IsSuccessful { get; set; }
        public int ScoreAwarded { get; set; } // This might belong in the Problems table, not sure
        public int? ExecutionTime { get; set; } // ET and MU might be worth scrapping, we'll see as we go on
        public int? MemoryUsage { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Problem Problem { get; set; }
        public Language Language { get; set; }
    }
}
