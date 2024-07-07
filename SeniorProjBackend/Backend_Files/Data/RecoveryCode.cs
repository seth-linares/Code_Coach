namespace SeniorProjBackend.Data
{
    public class RecoveryCode
    {
        /*
        11. **RecoveryCodes Table**:

            *   `RecoveryCodeID` (Primary Key, INT): Unique identifier for each recovery code.
            *   `UserID` (INT, Foreign Key, references `Users.UserID`): The user to whom the recovery code belongs.
            *   `Code` (VARCHAR): Hashed recovery code.
            *   `CreationDate` (DateTime): The date and time when the recovery code was generated.
        */
        public int RecoveryCodeID { get; set; }
        public int UserId { get; set; } // Foreign Key; Users.UserId
        public string Code { get; set; } // needs to be hashed
        public DateTime CreationDate { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
