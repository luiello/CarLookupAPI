using CarLookup.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware.Handlers;

/// <summary>
/// Default fallback handler for unhandled exceptions (500)
/// </summary>
public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DefaultExceptionHandler> _logger;

    public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles all exceptions as fallback (should be registered last)
    /// </summary>
    public bool CanHandle(Exception exception) => true;

    public Task<(int StatusCode, ApiResponse<object> Response)> HandleAsync(Exception exception, HttpContext context)
    {
        _logger.LogError(exception, "Unhandled exception occurred during request processing");

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "An internal server error occurred",
            Meta = new ResponseMeta
            {
                RequestId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            },
            Error = new ErrorDetails
            {
                Code = (int)HttpStatusCode.InternalServerError,
                Type = "InternalServerError",
                Details = new ErrorDetailInfo
                {
                    Message = GetSafeErrorMessage(exception)
                }
            }
        };

        return Task.FromResult(((int)HttpStatusCode.InternalServerError, response));
    }

    /// <summary>
    /// Gets a safe error message that doesn't expose sensitive information in production
    /// </summary>
    private static string GetSafeErrorMessage(Exception exception)
    {
        // In production, we don't want to expose internal exception details
        // In development, it might be helpful to see the actual exception message
        #if DEBUG
            return $"Internal server error: {exception.Message}";
        #else
            return "The server encountered an unexpected condition that prevented it from fulfilling the request";
        #endif
    }
}