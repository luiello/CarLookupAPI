using CarLookup.Contracts.Responses;
using CarLookup.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware.Handlers;

/// <summary>
/// Handler for ConflictDomainException (409)
/// </summary>
public class ConflictExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is ConflictDomainException;

    public Task<(int StatusCode, ApiResponse<object> Response)> HandleAsync(Exception exception, HttpContext context)
    {
        var conflictEx = (ConflictDomainException)exception;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Conflict occurred",
            Meta = new ResponseMeta
            {
                RequestId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            },
            Error = new ErrorDetails
            {
                Code = (int)HttpStatusCode.Conflict,
                Type = "ConflictError",
                Details = new ErrorDetailInfo
                {
                    Message = conflictEx.Message
                }
            }
        };

        return Task.FromResult(((int)HttpStatusCode.Conflict, response));
    }
}