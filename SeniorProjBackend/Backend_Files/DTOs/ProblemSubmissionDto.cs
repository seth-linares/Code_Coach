namespace SeniorProjBackend.DTOs
{
    public class SuccessfulSubmissionDto
    {
        public int SubmissionId { get; set; }
        public int ProblemId { get; set; }
        public string ProblemTitle { get; set; }
        public string LanguageName { get; set; }
        public string SubmittedCode { get; set; }
        public DateTimeOffset SubmissionTime { get; set; }
        public float? ExecutionTime { get; set; }
        public float? MemoryUsed { get; set; }
    }
    public class SubmissionBaseDto
    {
        public int SubmissionId { get; set; }
        public int ProblemId { get; set; }
        public string ProblemTitle { get; set; }
        public string LanguageName { get; set; }
        public DateTimeOffset SubmissionTime { get; set; }
        public bool IsSuccessful { get; set; }
        public float? ExecutionTime { get; set; }
        public float? MemoryUsed { get; set; }
    }

    // DTO for detailed submission information
    public class SubmissionDetailsDto : SubmissionBaseDto
    {
        public string SubmittedCode { get; set; }
    }

    // DTO for problem submission list items
    public class ProblemSubmissionDto : SubmissionBaseDto
    {
        // This now includes all fields from SubmissionBaseDto
    }

    // DTO for recent submissions (excludes execution details)
    public class RecentSubmissionDto
    {
        public int SubmissionId { get; set; }
        public int ProblemId { get; set; }
        public string ProblemTitle { get; set; }
        public string LanguageName { get; set; }
        public DateTimeOffset SubmissionTime { get; set; }
        public bool IsSuccessful { get; set; }
    }

    // Request DTO for submitting code
    public class SubmissionRequestDto
    {
        public string EncodedCode { get; set; } // base64
        public int ProblemId { get; set; }
        public int Judge0LanguageId { get; set; } // 51 = C#, 54 = C++, 94 = Python
    }

    // Response DTO for paginated problem submissions
    public class PaginatedProblemSubmissionsResponse
    {
        public int ProblemId { get; set; }
        public string ProblemTitle { get; set; }
        public List<ProblemSubmissionDto> Submissions { get; set; }
    }

    // Request DTO for fetching problem submissions
    public class ProblemSubmissionsRequest
    {
        public int ProblemId { get; set; }
    }
}
