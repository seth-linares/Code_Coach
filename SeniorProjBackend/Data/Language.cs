namespace SeniorProjBackend.Data
{
    public class Language
    {
        /*
         * 4.  **Languages Table**:
    
                *   `LanguageID` (Primary Key, INT).
                *   `LanguageName` (VARCHAR).
        */
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }

        // Navigation Properties
        public List<ProblemLanguage> ProblemLanguages { get; set; }

    }
}
