using System.Collections.Generic;

namespace SeniorProjBackend.Data
{
    public class RecoveryCodes
    {
        /*
        11. **RecoveryCodes Table**:

            *   `RecoveryCodeID` (Primary Key, INT): Unique identifier for each recovery code.
            *   `UserID` (INT, Foreign Key, references `Users.UserID`): The user to whom the recovery code belongs.
            *   `Code` (VARCHAR): Hashed recovery code.
            *   `CreationDate` (DateTime): The date and time when the recovery code was generated.
        */
        public int RecoveryCodeID { get; set; }
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public string Code { get; set; } // needs to be hashed
        public DateTime CreationDate { get; set; }
    }
}
