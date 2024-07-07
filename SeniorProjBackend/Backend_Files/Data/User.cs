using Microsoft.AspNetCore.Identity;

namespace SeniorProjBackend.Data
{
    public class User : IdentityUser<int>
    {
        // Inherited from IdentityUser:
        // public int Id { get; set; }                     // Replaces UserID
        // public string UserName { get; set; }            // Replaces Username
        // public string Email { get; set; }               // Replaces EmailAddress
        // public bool TwoFactorEnabled { get; set; }      // Already part of IdentityUser
        // public string PasswordHash { get; set; }        // Handled internally by Identity

        // Additional fields provided by IdentityUser:
        // public string PhoneNumber { get; set; }
        // public bool PhoneNumberConfirmed { get; set; }
        // public bool EmailConfirmed { get; set; }
        // public bool LockoutEnabled { get; set; } 
        // public DateTimeOffset? LockoutEnd { get; set; }
        // public int AccessFailedCount { get; set; }
        // public string ConcurrencyStamp { get; set; }
        // public string NormalizedEmail { get; set; }
        // public string NormalizedUserName { get; set; }
        // public string SecurityStamp { get; set; }

        // Custom properties
        public int TotalScore { get; set; }
        public string ProfilePictureURL { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastActiveDate { get; set; }
        public string Rank { get; set; }
        public int ActiveStreak { get; set; }

        // Removed: SecretKey is now handled internally by Identity for 2FA

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