using CarLookup.Infrastructure.Services.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CarLookup.Infrastructure.Services;

/// <summary>
/// Service for password hashing and verification using PBKDF2
/// </summary>
public class PasswordService : IPasswordService
{
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 10000; // PBKDF2 iterations

    /// <summary>
    /// Hash a password with a salt
    /// </summary>
    public string HashPassword(string password, string salt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(salt);

        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Generate a random salt
    /// </summary>
    public string GenerateSalt()
    {
        var saltBytes = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    public bool VerifyPassword(string password, string salt, string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(salt);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        try
        {
            var computedHash = HashPassword(password, salt);
            return computedHash == hash;
        }
        catch
        {
            return false;
        }
    }
}