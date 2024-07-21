using SeniorProjBackend.Data;
using System.Text.Json.Serialization;

namespace SeniorProjBackend.DTOs
{
    public class AddProblemRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
    }

    public class AddLanguageRequest
    {
        public string Name { get; set; }
        public int Judge0ID { get; set; }
    }

    public class AddProblemLanguageRequest
    {
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; }
        public string TestCode { get; set; }
    }

    public class ProblemLanguageDto
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; }
        public string TestCode { get; set; }
    }

    public class ProblemListRequest
    {
        public string Difficulty { get; set; }
        public string Category { get; set; }

        [JsonIgnore]
        public ProblemCategory? ParsedCategory { get; set; }
        [JsonIgnore]
        public DifficultyLevel? ParsedDifficulty { get; set; }
    }



    public class ProblemLanguageListRequest
    {
        public int? ProblemID { get; set; }
        public int? LanguageID { get; set; }
    }

    public class ProblemListItemDto
    {
        public int ProblemID { get; set; }
        public bool IsCompleted { get; set; }
        public string Title { get; set; }
        public string Difficulty { get; set; }

        [JsonIgnore]
        public DifficultyLevel? ParsedDifficulty { get; set; }
        public string Category { get; set; }

        [JsonIgnore]
        public ProblemCategory? ParsedCategory { get; set; }
        public int Points { get; set; }
    }

    public class LanguageListItemDto
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }
        public int Judge0ID { get; set; }
    }

    public class ProblemLanguageListItemDto
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; }
    }

    public class ProblemDetailDto
    {
        public int ProblemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }
        public List<string> AvailableLanguages { get; set; }
    }

    public class LanguageDetailDto
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }
        public int Judge0ID { get; set; }
    }

    public class ProblemLanguageDetailDto
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public string ProblemTitle { get; set; }
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }
        public string FunctionSignature { get; set; }
        public string TestCode { get; set; }
    }

    public class ProblemsByCategoryRequest
    {
        public string Category { get; set; }

        [JsonIgnore]
        public ProblemCategory? ParsedCategory { get; set; }
    }

    public class ProblemsByDifficultyRequest
    {
        public string Difficulty { get; set; }
    }

    public class FilteredProblemListItemDto : ProblemListItemDto
    {
        public bool IsCompleted { get; set; }
    }
}
