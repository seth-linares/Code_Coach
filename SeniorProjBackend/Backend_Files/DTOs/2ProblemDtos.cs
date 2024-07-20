using SeniorProjBackend.Data;

namespace SeniorProjBackend.DTOs
{

    public class ProblemDetailsDto
    {
        public int ProblemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }
        public int Points { get; set; }
        public List<ProblemLanguageDetailsDto> LanguageDetails { get; set; }
    }

    public class ProblemLanguageDetailsDto
    {
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }
        public int Judge0LanguageId { get; set; }
        public string FunctionSignature { get; set; }
    }

}
