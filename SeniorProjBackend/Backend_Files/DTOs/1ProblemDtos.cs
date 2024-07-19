using SeniorProjBackend.Data;

namespace SeniorProjBackend.DTOs
{
    public class AddProblemRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }
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

    public class ProblemListRequest : PaginationRequest
    {
        public DifficultyLevel? Difficulty { get; set; }
        public ProblemCategory? Category { get; set; }
    }

    public class ProblemLanguageListRequest : PaginationRequest
    {
        public int? ProblemID { get; set; }
        public int? LanguageID { get; set; }
    }

    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class ProblemListItemDto
    {
        public int ProblemID { get; set; }
        public string Title { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }
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

    public class ProblemsByCategoryRequest : PaginationRequest
    {
        public ProblemCategory Category { get; set; }
    }

    public class ProblemsByDifficultyRequest : PaginationRequest
    {
        public DifficultyLevel Difficulty { get; set; }
    }

    public class FilteredProblemListItemDto : ProblemListItemDto
    {
        public bool IsCompleted { get; set; }
    }
}
