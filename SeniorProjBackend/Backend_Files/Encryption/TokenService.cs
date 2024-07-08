using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using SeniorProjBackend.Data;

namespace SeniorProjBackend.Encryption
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        static string GenerateSecureKey() => "DEFAULT GENERATED";
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly string _key;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(_key))
            {
                _key = GenerateSecureKey();
                _logger.LogWarning("JWT Key not found in configuration. Generated new key.");
            }
            else
            {
                _logger.LogInformation("Using JWT Key from configuration.");
            }
        }

        public static string GenerateSecureKey()
        {
            byte[] randomBytes = new byte[32]; // 256 bits for HMAC SHA-256
            RandomNumberGenerator.Fill(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(20),
                signingCredentials: credentials
            );

            _logger.LogInformation($"\n\n\n\nToken: {token}\n\n\n\n");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                _logger.LogInformation("\n\n\n\nReturning principal\n\n\n\n");
                return principal;
            }
            catch
            {
                // If token validation fails, return null
                _logger.LogError("\n\n\n\nValidation failed\n\n\n\n");
                return null;
            }
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_key)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(2),
            };
        }
    }
}