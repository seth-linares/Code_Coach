using Microsoft.EntityFrameworkCore;

namespace SeniorProjBackend.Data
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string EmailAddress { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? SecretKey { get; set; }
        public int TotalScore { get; set; }
        public string? Bio {  get; set; }
        public string? ProfilePictureURL { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastActiveDate { get; set; }
        public string Rank { get; set; }
        public string RankIconURL { get; set; }
        public int ActiveStreak { get; set; }

    }
}
