using SeniorProjBackend.Data;
using System.Text.Json.Serialization;

namespace SeniorProjBackend.DTOs
{
    public class AddProblemRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Points { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class AddLanguageRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Judge0ID { get; set; }
    }

    public class AddProblemLanguageRequest
    {
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; } = string.Empty;
        public string TestCode { get; set; } = string.Empty;
    }

    public class ProblemLanguageDto
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; } = string.Empty;
        public string TestCode { get; set; } = string.Empty;
    }

    public class ProblemListRequest
    {
        public string Difficulty { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

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
        public string Title { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;

        [JsonIgnore]
        public DifficultyLevel? ParsedDifficulty { get; set; }
        public string Category { get; set; } = string.Empty;

        [JsonIgnore]
        public ProblemCategory? ParsedCategory { get; set; }
        public int Points { get; set; }
    }

    public class LanguageListItemDto
    {
        public int LanguageID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Judge0ID { get; set; }
    }

    public class ProblemLanguageListItemDto
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string FunctionSignature { get; set; } = string.Empty;
    }

    public class ProblemDetailDto
    {
        public int ProblemID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Points { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }
        public List<string> AvailableLanguages { get; set; } = new List<string>();
    }

    public class LanguageDetailDto
    {
        public int LanguageID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Judge0ID { get; set; }
    }

    public class ProblemLanguageDetailDto
    {
        public int ProblemLanguageID { get; set; }
        public int ProblemID { get; set; }
        public string ProblemTitle { get; set; } = string.Empty;
        public int LanguageID { get; set; }
        public string LanguageName { get; set; } = string.Empty;
        public string FunctionSignature { get; set; } = string.Empty;
        public string TestCode { get; set; } = string.Empty;
    }

    public class ProblemsByCategoryRequest
    {
        public string Category { get; set; } = string.Empty;

        [JsonIgnore]
        public ProblemCategory? ParsedCategory { get; set; }
    }

    public class ProblemsByDifficultyRequest
    {
        public string Difficulty { get; set; } = string.Empty;
    }

    public class FilteredProblemListItemDto : ProblemListItemDto
    {
        public bool IsCompleted { get; set; }
    }
}
