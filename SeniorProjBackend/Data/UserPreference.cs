namespace SeniorProjBackend.Data
{
    public class UserPreference // Might be able to scrap this table tbh. Preferences can likely be done locally on frontend?
    {
        /*
         8.  **UserPreferences Table**:
    
            *   `UserPreferenceID` (Primary Key, INT).
            *   `UserID` (INT, Foreign Key, references `Users.UserID`).
            *   `PreferenceKey` (VARCHAR) represents the type or name of the preference (e.g., "Language", "Theme", "Notification", etc.)
            *   `PreferenceValue` (VARCHAR) holds the value of the preference (e.g., "en" for English language, "dark" for dark theme, "true" for enabling notifications, etc.)
         */
        public int UserPreferenceID { get; set; }
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public string PreferenceKey { get; set; } 
        public string PreferenceValue { get; set; } 

        // Navigation Properties
        public User User { get; set; }
    }
}
