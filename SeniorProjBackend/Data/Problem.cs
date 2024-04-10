namespace SeniorProjBackend.Data
{
    public class Problem
    {
        /*
         * 2.  **Problems Table**:
                *   `ProblemID` (Primary Key, INT): Unique identifier for each problem.
                *   `Title` (VARCHAR): Title of the problem.
                *   `Description` (NVARCHAR(MAX)): Detailed description of the problem.
                *   `DifficultyScore` (INT): ELO/Kyu score indicating the problem's difficulty.
                *   `IsActive` (Boolean): Indicates whether the problem is active and available to users.
                *   `LastModifiedDate` (DateTime): Date and time when the problem was last modified.
                *   `TestCodeFileName` (VARCHAR): Filename of the file containing the test code for the problem.
        */

        public int ProblemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // NVARCHAR MAX
        public int DifficultyScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string TestCodeFileName { get; set; }

        // Navigation properties
        public List<AIConversation> AIConversations { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public List<ProblemCategory> ProblemCategories { get; set; }
        public List<ProblemLanguage> ProblemLanguages { get; set; }
        
    }
}
