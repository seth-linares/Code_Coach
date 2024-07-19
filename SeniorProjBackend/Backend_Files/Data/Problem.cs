namespace SeniorProjBackend.Data
{
    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
    }

    public enum ProblemCategory
    {
        WarmUps = 1,
        Strings = 2,
        Booleans = 3,
        Lists = 4,
    }

    public class Problem
    {
        public int ProblemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public ProblemCategory Category { get; set; }

        // Navigation properties
        public List<ProblemLanguage> ProblemLanguages { get; set; }
        public List<UserSubmission> UserSubmissions { get; set; }
        public List<AIConversation> AIConversations { get; set; }
    }
}