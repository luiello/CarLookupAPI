using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Manager.Interfaces;

/// <summary>
/// Authentication manager interface
/// </summary>
public interface IAuthManager
{
    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    Task<TokenResponse> AuthenticateAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);
}
