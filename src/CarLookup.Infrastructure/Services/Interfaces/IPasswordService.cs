namespace CarLookup.Infrastructure.Services.Interfaces;

/// <summary>
/// Service interface for password hashing and verification
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hash a password with a salt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="salt">Salt for hashing</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password, string salt);

    /// <summary>
    /// Generate a random salt
    /// </summary>
    /// <returns>Base64 encoded salt</returns>
    string GenerateSalt();

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="salt">Salt used for hashing</param>
    /// <param name="hash">Stored hash to verify against</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string salt, string hash);
}