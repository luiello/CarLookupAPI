using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using CarLookup.Manager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Host.Controllers;

/// <summary>
/// Authentication controller for JWT token management
/// </summary>
[Route("api/v1/[controller]")]
[Tags("Authentication")]
public class AuthController : BaseApiController
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
    {
        _authManager = authManager;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token response</returns>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> GetTokenAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authentication attempt for username: {Username}", request.Username);
        
        var result = await _authManager.AuthenticateAsync(request, cancellationToken);
        
        _logger.LogInformation("Authentication successful for username: {Username}", request.Username);
        
        return SuccessResponse(result, "Authentication successful");
    }
}