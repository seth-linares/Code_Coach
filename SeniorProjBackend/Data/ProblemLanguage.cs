namespace SeniorProjBackend.Data
{
    public class ProblemLanguage
    {

        /*
         * 6.  **ProblemLanguages Junction Table**:
    
                *   `ProblemLanguageID` (Primary Key, INT).
                *   `ProblemID` (INT, Foreign Key, references `Problems.ProblemID`).
                *   `LanguageID` (INT, Foreign Key, references `Languages.LanguageID`).
         */
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; } // Foreign Key; Problems.ProblemID
        public int LanguageID { get; set; } // Foreign Key; Languages.LanguageID

        // Navigation Properties
        public Problem Problem { get; set; }
        public Language Language { get; set; }
    }
}
