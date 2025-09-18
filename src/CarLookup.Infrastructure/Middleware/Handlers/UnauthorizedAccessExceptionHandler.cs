using CarLookup.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware.Handlers;

/// <summary>
/// Handler for UnauthorizedAccessException (401)
/// </summary>
public class UnauthorizedAccessExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is UnauthorizedAccessException;

    public Task<(int StatusCode, ApiResponse<object> Response)> HandleAsync(Exception exception, HttpContext context)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Access denied",
            Meta = new ResponseMeta
            {
                RequestId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            },
            Error = new ErrorDetails
            {
                Code = (int)HttpStatusCode.Unauthorized,
                Type = "UnauthorizedError",
                Details = new ErrorDetailInfo
                {
                    Message = "You do not have permission to access this resource"
                }
            }
        };

        return Task.FromResult(((int)HttpStatusCode.Unauthorized, response));
    }
}