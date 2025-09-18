using System.Collections.Generic;

namespace CarLookup.Infrastructure.Services.Interfaces;

/// <summary>
/// JWT token service interface
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate JWT token for user
    /// </summary>
    string GenerateToken(string username, IEnumerable<string> roles);

    /// <summary>
    /// Calculates the expiration time in seconds based on the configured JWT expiration duration.
    /// </summary>
    /// <returns>The expiration time in seconds as an integer.</returns>
    int GetExpirationSeconds();

    /// <summary>
    /// Validate JWT token
    /// </summary>
    bool ValidateToken(string token);
}