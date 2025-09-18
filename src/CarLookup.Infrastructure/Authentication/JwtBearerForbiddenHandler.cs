using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Authentication;

/// <summary>
/// Handles JWT Bearer forbidden events (403 Forbidden responses)
/// </summary>
public class JwtBearerForbiddenHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Handles authorization failures by returning a structured JSON response with role requirements
    /// </summary>
    public static async Task HandleForbiddenAsync(ForbiddenContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var detailsMessage = ExtractRoleRequirements(context.HttpContext);

        var response = new
        {
            success = false,
            message = "Access denied - insufficient permissions",
            meta = new
            {
                requestId = context.HttpContext.TraceIdentifier,
                timestamp = DateTime.UtcNow
            },
            error = new
            {
                code = StatusCodes.Status403Forbidden,
                type = "ForbiddenError",
                details = new
                {
                    message = detailsMessage
                }
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    /// <summary>
    /// Extracts role requirements from endpoint metadata to provide detailed error messages
    /// </summary>
    private static string ExtractRoleRequirements(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Enumerable.Empty<IAuthorizeData>();
        
        var roles = authorizeData
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .SelectMany(a => a.Roles!.Split(','))
            .Select(r => r.Trim())
            .Distinct()
            .ToArray();

        if (roles.Length > 0)
        {
            var roleList = string.Join(", ", roles.Select(role => $"({role})"));
            return $"Authorization failed. User must be part of one of the following roles: {roleList}";
        }

        return "Authorization failed. Insufficient permissions to access this resource.";
    }
}