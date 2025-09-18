using CarLookup.Infrastructure.Middleware.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware;

/// <summary>
/// Global exception handling middleware that delegates to registered handlers
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IEnumerable<IExceptionHandler> handlers)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, exception, handlers);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, IEnumerable<IExceptionHandler> handlers)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(
                "Cannot write error response, response has already started. Exception: {ExceptionType}: {ExceptionMessage}",
                exception.GetType().Name,
                exception.Message);
            
            return;
        }

        var response = context.Response;
        var handler = handlers.FirstOrDefault(h => h.CanHandle(exception));

        if (handler == null)
        {
            _logger.LogWarning("No exception handler found for exception type: {ExceptionType}", exception.GetType().Name);
            handler = handlers.Last(); // DefaultExceptionHandler should be last and handle all exceptions
        }

        var (statusCode, errorResponse) = await handler.HandleAsync(exception, context);

        response.StatusCode = statusCode;
        response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await response.WriteAsync(jsonResponse);
    }
}