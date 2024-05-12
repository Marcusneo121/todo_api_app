using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace todo_api_app.Utils;

public static class AuthUtils
{
    public static byte[] SaltProvider()
    {
        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        Console.WriteLine($"Ori Salt: {Convert.ToBase64String(salt)}");

        return salt;
    }

    public static string PasswordHashing(string password, byte[] salt)
    {
        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        Console.WriteLine($"Passed In Salte: {Convert.ToBase64String(salt)}");
        Console.WriteLine($"Hashed: {hashed}");

        return hashed;
    }

    public static void VerifyHashedPassword(string hashedpassword, string userInputPassword)
    {
    }
}