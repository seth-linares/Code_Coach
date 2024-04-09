using System.Security.Cryptography;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

// Might scrap since we want to use KMS and I don't think we need to store the keys in the appsettings.json

namespace SeniorProjBackend.Encryption
{


    public struct AppSettings
    {

        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public EncryptionKeys EncryptionKeys { get; set; }


    }

    public struct Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public struct LogLevel
    {
        public string Default { get; set; }
        [JsonPropertyName("Microsoft.AspNetCore")]
        public string Microsoft_AspNetCore { get; set; }
    }

    public struct ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public struct EncryptionKeys
    {
        public string SecretKeyEK { get; set; }
        public string APIKeyEK { get; set; }
    }
    public class EncryptionKey
    {
        /*
         * Need a method to generate the encryption keys for 2FA Secret Key and API Key for AI
         * 
         * Need to have a method that rotates the keys out and re-encrypts with new key
         * Both need to update appsettings.json
         * 
         */
        // Need a gen method

        // Need a rotate method

        


        public async Task GenerateEncryptionKey()
        {
            var json_string = await File.ReadAllTextAsync(@"SeniorProjBackend\appsettings.json");
            AppSettings AppSettings = JsonSerializer.Deserialize<AppSettings>(json_string);
            
        }


    }
}
