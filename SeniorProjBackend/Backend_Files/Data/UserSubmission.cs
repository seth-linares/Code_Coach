namespace SeniorProjBackend.Data
{
    public class UserSubmission
    {
        public int SubmissionID { get; set; }
        public int UserId { get; set; }
        public int ProblemID { get; set; }
        public int LanguageID { get; set; }
        public string SubmittedCode { get; set; }
        public DateTimeOffset SubmissionTime { get; set; }
        public bool IsSuccessful { get; set; }
        public string Token { get; set; } // Judge0 token for later reference
        public float? ExecutionTime { get; set; } // Judge0 "time" field: Program’s run time in seconds; Float
        public float? MemoryUsed { get; set; } // Judge0 "memory" field: Memory used in kilobytes; Float.

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual Language Language { get; set; }
    }
}
