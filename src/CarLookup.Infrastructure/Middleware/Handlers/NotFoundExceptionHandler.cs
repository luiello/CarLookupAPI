using CarLookup.Contracts.Responses;
using CarLookup.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware.Handlers;

/// <summary>
/// Handler for NotFoundDomainException (404)
/// </summary>
public class NotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is NotFoundDomainException;

    public Task<(int StatusCode, ApiResponse<object> Response)> HandleAsync(Exception exception, HttpContext context)
    {
        var notFoundEx = (NotFoundDomainException)exception;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Resource not found",
            Meta = new ResponseMeta
            {
                RequestId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            },
            Error = new ErrorDetails
            {
                Code = (int)HttpStatusCode.NotFound,
                Type = "NotFoundError",
                Details = new ErrorDetailInfo
                {
                    Message = notFoundEx.Message
                }
            }
        };

        return Task.FromResult(((int)HttpStatusCode.NotFound, response));
    }
}