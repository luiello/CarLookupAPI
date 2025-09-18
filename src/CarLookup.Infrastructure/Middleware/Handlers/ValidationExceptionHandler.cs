using CarLookup.Contracts.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CarLookup.Infrastructure.Middleware.Handlers;

/// <summary>
/// Handler for BadRequest/ValidationException (400)
/// </summary>
public class ValidationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is ValidationException;

    public Task<(int StatusCode, ApiResponse<object> Response)> HandleAsync(Exception exception, HttpContext context)
    {
        var validationEx = (ValidationException)exception;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Meta = new ResponseMeta
            {
                RequestId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            },
            Error = new ErrorDetails
            {
                Code = (int)HttpStatusCode.BadRequest,
                Type = "ValidationError",
                Details = new ErrorDetailInfo
                {
                    Field = validationEx.Errors.FirstOrDefault()?.PropertyName,
                    Message = string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))
                }
            }
        };

        return Task.FromResult(((int)HttpStatusCode.BadRequest, response));
    }
}