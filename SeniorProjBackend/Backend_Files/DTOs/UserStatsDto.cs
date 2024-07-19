namespace SeniorProjBackend.DTOs
{
    public class UserStatsDto
    {
        public string Username { get; set; }
        public int TotalScore { get; set; }
        public string Rank { get; set; }
        public int CompletedProblems { get; set; }
        public int AttemptedProblems { get; set; }
        public string ProfilePictureURL { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
