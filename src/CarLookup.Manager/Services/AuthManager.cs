using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using CarLookup.Domain.Interfaces;
using CarLookup.Infrastructure.Services.Interfaces;
using CarLookup.Manager.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Manager.Services;

/// <summary>
/// Authentication manager implementation
/// </summary>
public class AuthManager : IAuthManager
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<LoginRequest> _validator;
    private readonly ILogger<AuthManager> _logger;

    public AuthManager(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IValidator<LoginRequest> validator,
        ILogger<AuthManager> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<TokenResponse> AuthenticateAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authenticating user: {Username}", loginRequest.Username);

        var validationResult = await _validator.ValidateAsync(loginRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Login request validation failed for username: {Username}", loginRequest.Username);
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userRepository.GetByUsernameAsync(loginRequest.Username, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Authentication failed for username: {Username} - User not found", loginRequest.Username);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var isValidPassword = _passwordService.VerifyPassword(loginRequest.Password, user.Salt, user.PasswordHash);
        if (!isValidPassword)
        {
            _logger.LogWarning("Authentication failed for username: {Username} - Invalid password", loginRequest.Username);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Authentication failed for username: {Username} - User account is inactive", loginRequest.Username);
            throw new UnauthorizedAccessException("Account is inactive");
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        var token = _jwtTokenService.GenerateToken(user.Username, roles);

        _logger.LogInformation("Authentication successful for username: {Username} with roles: {Roles}", 
            loginRequest.Username, string.Join(", ", roles));

        return new TokenResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = _jwtTokenService.GetExpirationSeconds(),
            Roles = roles
        };
    }
}
