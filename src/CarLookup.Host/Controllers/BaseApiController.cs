using CarLookup.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace CarLookup.Host.Controllers;

/// <summary>
/// Base controller for API responses with consistent envelope format
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Creates a successful API response with data
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>Wrapped API response</returns>
    protected ActionResult<ApiResponse<T>> SuccessResponse<T>(T data, string message = "Operation completed successfully")
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Meta = new ResponseMeta
            {
                RequestId = HttpContext.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a successful API response for created resources
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="data">Response data</param>
    /// <param name="actionName">Action name for location header</param>
    /// <param name="routeValues">Route values for location header</param>
    /// <param name="message">Success message</param>
    /// <returns>Wrapped API response with 201 status</returns>
    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(
        T data, 
        string actionName, 
        object routeValues = null, 
        string message = "Resource created successfully")
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Meta = new ResponseMeta
            {
                RequestId = HttpContext.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            }
        };

        // Use Created() with a manually constructed URI instead of CreatedAtAction
        if (routeValues != null)
        {
            var routeValueDict = new RouteValueDictionary(routeValues);
            var location = Url.Action(actionName, routeValueDict);
            if (!string.IsNullOrEmpty(location))
            {
                return Created(location, response);
            }
        }
        
        // Fallback to simple Created response if URL generation fails
        return StatusCode(201, response);
    }

    /// <summary>
    /// Creates a successful API response with no content
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Wrapped API response with 200 status and no data</returns>
    protected ActionResult<ApiResponse<object>> SuccessResponse(string message = "Operation completed successfully")
    {
        var response = new ApiResponse<object>
        {
            Success = true,
            Message = message,
            Data = null,
            Meta = new ResponseMeta
            {
                RequestId = HttpContext.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            }
        };

        return Ok(response);
    }
}