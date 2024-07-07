namespace SeniorProjBackend.Data
{

    // MIGHT NOT NEED THIS ANYMORE!!!! POSSIBLY FREE CHATGPT USE https://github.com/PawanOsman/ChatGPT
    public class APIKey
    {
        /*
        12. **APIKeys Table** (New):
        * `APIKeyID` (Primary Key, INT): Unique identifier for each API key.
        * `UserID` (INT, Foreign Key, references `Users.UserID`): The user to whom the API key belongs.
        * `KeyType` (VARCHAR): Type of the API key (e.g., 'ChatGPT', 'Gemini').
        * `KeyValue` (VARCHAR): The actual API key, encrypted for security.
        * `Permissions` (VARCHAR, optional): Describes the scope or permissions granted by the API key. 
        * `CreatedAt` (DateTime): Timestamp of when the key was created.
        * `ExpiresAt` (DateTime, optional): Timestamp of when the key expires, if applicable.
        */

        public int APIKeyID { get; set; }
        public int UserId { get; set; } // Foreign Key; Users.UserId
        public string KeyType { get; set; }
        public string KeyValue { get; set; }
        public string? Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set;}

        // Navigation Properties
        public User User { get; set; }  

    }
}
