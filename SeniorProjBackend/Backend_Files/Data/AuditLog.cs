namespace SeniorProjBackend.Data
{
    public enum EventType
    {
        Login,
        TwoFactorAuth,
        APIKeyUsage,
        PasswordChange,
        PasswordReset,
        AccountCreation,
        AccountDeletion,
        RecoveryCodeUsage,
        RecoveryCodeChange,
        AccountLockout,
        AccountUnlock,
        AccountUpdate,
        AccountEmailUpdate,
        JWTTokenCreation,
        JWTTokenVerification,
        InvalidLoginAttempt,
        InvalidRecoveryCodeAttempt,
        InvalidAPIKeyAttempt,
        InvalidTwoFactorAuthAttempt,
        PasswordResetRequest,
 
    }
    public class AuditLog
    {
        /*
        13. **AuditLogs Table** (New):
            * `AuditLogID` (Primary Key, INT): Unique identifier for each log entry.
            * `UserID` (INT, Foreign Key, references `Users.UserID`, nullable): The user associated with the event, if applicable.
            * `EventType` (VARCHAR): The type of event logged (e.g., login, 2FA verification, API key usage).
            * `Details` (VARCHAR(MAX)): Detailed information about the event, potentially stored as JSON for structured data.
            * `Timestamp` (DateTime): The date and time when the event occurred.
        */

        public int AuditLogID { get; set; }
        public int? UserID { get; set; } // Foreign Key; Users.UserID
        public EventType EventType { get; set; }
        public string Details { get; set; } // Varchar Max
        public DateTime Timestamp { get; set; }

        // Navigation Properties
        public User? User { get; set; }

    }
}
