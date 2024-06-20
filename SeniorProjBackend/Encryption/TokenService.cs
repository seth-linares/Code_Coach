using Microsoft.IdentityModel.Tokens;
using SeniorProjBackend.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace SeniorProjBackend.Encryption
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        static string GenerateSecureKey() => "DEFAULT GENERATED";
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GenerateSecureKey()
        {
            byte[] randomBytes = new byte[32]; // 256 for HMAC SHA-256
            RandomNumberGenerator.Fill(randomBytes);
            string byteString = Encoding.UTF8.GetString(randomBytes);

            return byteString;
        }

        public string GenerateToken(User user)
        {
            var potential_key = TokenService.GenerateSecureKey();
            string key = _configuration["EncryptionKeys:JwtKey"] ?? potential_key;
            
            if(key == potential_key)
                Console.WriteLine($"New Key generated: {potential_key}");
            else 
                Console.WriteLine($"Key From File: {key}");
            
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(20), // 20 hours of authentication
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }


}
