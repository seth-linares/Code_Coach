using Microsoft.AspNetCore.Identity;

namespace SeniorProjBackend.Data
{

    public enum Ranks
    {
        Newbie = 0,
        Novice = 25,
        Amateur = 75,
        Talented = 150,
        Pro = 300
    }
    public class User : IdentityUser<int>
    {
        // Implicit fields from IdentityUser<int>:
        // public virtual int Id { get; set; }
        // public virtual string UserName { get; set; }
        // public virtual string NormalizedUserName { get; set; }
        // public virtual string Email { get; set; }
        // public virtual string NormalizedEmail { get; set; }
        // public virtual bool EmailConfirmed { get; set; }
        // public virtual string PasswordHash { get; set; }
        // public virtual string SecurityStamp { get; set; }
        // public virtual string ConcurrencyStamp { get; set; }
        // public virtual string PhoneNumber { get; set; }
        // public virtual bool PhoneNumberConfirmed { get; set; }
        // public virtual bool TwoFactorEnabled { get; set; }
        // public virtual DateTimeOffset? LockoutEnd { get; set; }
        // public virtual bool LockoutEnabled { get; set; }
        // public virtual int AccessFailedCount { get; set; }

        // Custom properties
        public int TotalScore { get; set; }
        public string ProfilePictureURL { get; set; } // URL: https://cdn.pfps.gg/pfps/9150-cat-25.png
        public DateTimeOffset RegistrationDate { get; set; }
        public Ranks Rank { get; private set; }
        public int CompletedProblems { get; set; }
        public int AttemptedProblems { get; set; }

        // Navigation properties
        public virtual List<AIConversation> AIConversations { get; set; }
        public virtual List<APIKey> APIKeys { get; set; }
        public virtual List<UserSubmission> UserSubmissions { get; set; }

        public void UpdateRank()
        {
            Rank = CalculateRank(TotalScore);
        }

        public static Ranks CalculateRank(int score)
        {
            if (score >= 300) return Ranks.Pro;
            if (score >= 150) return Ranks.Talented;
            if (score >= 75) return Ranks.Amateur;
            if (score >= 25) return Ranks.Novice;
            return Ranks.Newbie;
        }

    }
}