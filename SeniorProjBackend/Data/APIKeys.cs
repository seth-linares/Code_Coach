using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

namespace SeniorProjBackend.Data
{
    public class APIKeys
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
        public string APIKeyID { get; set; }
        public int UserID { get; set; } // Foreign Key; Users.UserID
        public string KeyType { get; set; }
        public string KeyValue { get; set; }
        public string? Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set;}

    }
}
