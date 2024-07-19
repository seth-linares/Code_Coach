namespace SeniorProjBackend.Data
{
    public class ProblemLanguage
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; }
        public string TestCode { get; set; }

        // Navigation properties
        public Problem Problem { get; set; }
        public Language Language { get; set; }
    }
}
