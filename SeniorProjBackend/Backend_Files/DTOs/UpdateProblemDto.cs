using SeniorProjBackend.Data;

namespace SeniorProjBackend.DTOs
{
    public class UpdateProblemRequest
    {
        public int ProblemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }
    }

    public class UpdateLanguageRequest
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }
        public int Judge0ID { get; set; }
    }

    public class UpdateProblemLanguageRequest
    {
        public int ProblemLanguageID { get; set; }
        public string FunctionSignature { get; set; }
        public string TestCode { get; set; }
    }
}
