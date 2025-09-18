using CarLookup.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace CarLookup.Infrastructure.Authentication;

/// <summary>
/// Factory for creating JWT Bearer options with proper configuration
/// </summary>
public class JwtBearerOptionsFactory
{
    /// <summary>
    /// Creates and configures JWT Bearer options
    /// </summary>
    public static JwtBearerOptions Create(JwtOptions jwtOptions)
    {
        var options = new JwtBearerOptions();
        
        ConfigureTokenValidation(options, jwtOptions);
        ConfigureEvents(options);
        
        return options;
    }

    /// <summary>
    /// Configures token validation parameters
    /// </summary>
    private static void ConfigureTokenValidation(JwtBearerOptions options, JwtOptions jwtOptions)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    /// <summary>
    /// Configures JWT Bearer events for custom response handling
    /// </summary>
    private static void ConfigureEvents(JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = JwtBearerChallengeHandler.HandleChallengeAsync,
            OnForbidden = JwtBearerForbiddenHandler.HandleForbiddenAsync
        };
    }
}