using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public class PasswordHasher
{

    private const int ITERATIONS = 100000;
    public string HashPassword(string password)
    {
        // generate a 128-bit salt using a secure PRNG
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // get a 256-bit hash of the password
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: ITERATIONS,
            numBytesRequested: 256 / 8));

        // return the salt and the hash
        return $"{ITERATIONS}.{Convert.ToBase64String(salt)}.{hashed}";
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        // extract the iteration and salt from the hash
        var parts = hashedPassword.Split('.', 3);

        if (parts.Length != 3)
        {
            throw new FormatException("Unexpected hash format. Should be formatted as `{iterations}.{salt}.{hash}`");
        }

        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = parts[2];

        // hash the provided password using the same parameters
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: providedPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations,
            numBytesRequested: 256 / 8));

        return hash == expectedHash;
    }
}
