using System.Collections.Generic;
using System.Linq;

namespace CarLookup.Contracts.Responses;

/// <summary>
/// Token response for authentication
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token type (typically "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}