namespace SeniorProjBackend.Data
{
    public class UserPreference // Might be able to scrap this table tbh. Preferences can likely be done locally on frontend?
    {
        /*
         8.  **UserPreferences Table**:
    
            *   `UserPreferenceID` (Primary Key, INT).
            *   `UserID` (INT, Foreign Key, references `Users.UserID`).
            *   `PreferenceType` (VARCHAR).
            *   `PreferenceValue` (VARCHAR).
         */
        public int UserPreferenceID { get; set; }
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public string PreferenceType { get; set; } // WiP I suppose!
        public string PreferenceValue { get; set; } // WiP I suppose!

        // Navigation Properties
        public User User { get; set; }
    }
}
