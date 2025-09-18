using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Authentication;

/// <summary>
/// Handles JWT Bearer challenge events (401 Unauthorized responses)
/// </summary>
public class JwtBearerChallengeHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Handles authentication challenges by returning a structured JSON response
    /// </summary>
    public static async Task HandleChallengeAsync(JwtBearerChallengeContext context)
    {
        // Skip the default logic
        context.HandleResponse();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = "Authentication required",
            meta = new
            {
                requestId = context.HttpContext.TraceIdentifier,
                timestamp = DateTime.UtcNow
            },
            error = new
            {
                code = StatusCodes.Status401Unauthorized,
                type = "UnauthorizedError",
                details = new
                {
                    message = "A valid authentication token is required to access this resource."
                }
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}