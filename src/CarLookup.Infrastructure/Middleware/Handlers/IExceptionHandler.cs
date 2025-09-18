using CarLookup.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware.Handlers;

/// <summary>
/// Interface for handling specific exception types
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// Determines if this handler can process the given exception
    /// </summary>
    /// <param name="exception">The exception to evaluate</param>
    /// <returns>True if this handler can process the exception</returns>
    bool CanHandle(Exception exception);

    /// <summary>
    /// Handles the exception and produces an appropriate HTTP response
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="context">The current HTTP context</param>
    /// <returns>The HTTP status code and API response</returns>
    Task<(int StatusCode, ApiResponse<object> Response)> HandleAsync(Exception exception, HttpContext context);
}