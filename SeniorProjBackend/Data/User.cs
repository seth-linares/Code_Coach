namespace SeniorProjBackend.Data
{
    public class User
    {
        /*
         1.  **Users Table**:
            *   `UserID` (Primary Key, INT): Unique identifier for each user.
            *   `Username` (VARCHAR): User's chosen username.
            *   `PasswordHash` (VARCHAR): Hash of the user's password for secure storage.
            *   `EmailAddress` (VARCHAR): User's email address.
            *   `TwoFactorEnabled` (Boolean): Indicates if two-factor authentication is enabled.
            *   `SecretKey` (VARCHAR, optional): Encrypted  secret key used for generating 2FA codes. Null or empty if 2FA is not enabled.
            *   `TotalScore` (INT): Accumulated score from problem-solving activities.
            *   `Bio` (VARCHAR, optional): Short biography or user description.
            *   `ProfilePictureURL` (VARCHAR): URL to the user's profile picture, has default value
            *   `RegistrationDate` (DateTime): Date and time when the user registered.
            *   `LastActiveDate` (DateTime): Date and time when the user was last active.
            *   `Rank` (VARCHAR): User's rank, calculated from `TotalScore`.
            *   `RankIconURL` (VARCHAR): URL to an icon/image representing the user's rank, has default value
            *   `ActiveStreak` (INT, optional): Number of consecutive days the user has been active.
        */

        // delete either Rank or RankIconURL

        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string EmailAddress { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? SecretKey { get; set; } // encrypted secret key for 2FA
        public int TotalScore { get; set; }
        public string? Bio {  get; set; }
        public string ProfilePictureURL { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastActiveDate { get; set; }
        public string Rank { get; set; } 
        public string RankIconURL { get; set; } // For icons we need to have a default url for each in the fluent api
        public int ActiveStreak { get; set; }

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
