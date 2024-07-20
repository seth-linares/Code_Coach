namespace SeniorProjBackend.Data
{
    public class Language
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }
        public int Judge0ID { get; set; }

        // Navigation property
        public virtual List<ProblemLanguage> ProblemLanguages { get; set; }
    }
}
