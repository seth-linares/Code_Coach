using Microsoft.AspNetCore.Identity;

namespace SeniorProjBackend.Data
{
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
        public string? SecretKey { get; set; } // encrypted secret key for 2FA
        public int TotalScore { get; set; }
        public string ProfilePictureURL { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastActiveDate { get; set; }
        public string Rank { get; set; }

        // Navigation properties
        public List<AIConversation> AIConversations { get; set; }
        public List<APIKey> APIKeys { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public List<RecoveryCode> RecoveryCodes { get; set; }
        public List<UserPreference> UserPreferences { get; set; }
        public List<AuditLog> AuditLogs { get; set; }
        public List<UserSubmission> UserSubmissions { get; set; }
    }
}