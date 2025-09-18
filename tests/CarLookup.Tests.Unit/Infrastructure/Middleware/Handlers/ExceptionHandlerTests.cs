using CarLookup.Infrastructure.Middleware.Handlers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CarLookup.Tests.Unit.Infrastructure.Middleware.Handlers;

/// <summary>
/// Unit tests for exception handling middleware components.
/// Tests various exception handlers and their ability to handle different exception types.
/// </summary>
public class ExceptionHandlerTests
{
    /// <summary>
    /// Tests that ValidationExceptionHandler can identify ValidationException instances.
    /// Expects true when checking if handler can handle a ValidationException.
    /// </summary>
    [Fact]
    public void ValidationExceptionHandler_CanHandle_ValidationException_ReturnsTrue()
    {
        // Arrange
        var handler = new ValidationExceptionHandler();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };
        var exception = new ValidationException(validationFailures);

        // Act
        var canHandle = handler.CanHandle(exception);

        // Assert
        canHandle.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ValidationExceptionHandler cannot handle non-validation exceptions.
    /// Expects false when checking if handler can handle other exception types.
    /// </summary>
    [Fact]
    public void ValidationExceptionHandler_CanHandle_DifferentException_ReturnsFalse()
    {
        // Arrange
        var handler = new ValidationExceptionHandler();
        var exception = new ArgumentException("Test exception");

        // Act
        var canHandle = handler.CanHandle(exception);

        // Assert
        canHandle.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ValidationExceptionHandler produces correct error response format.
    /// Expects BadRequest status with structured validation error details.
    /// </summary>
    [Fact]
    public async Task ValidationExceptionHandler_HandleAsync_ReturnsCorrectResponse()
    {
        // Arrange
        var handler = new ValidationExceptionHandler();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Invalid email format")
        };
        var exception = new ValidationException(validationFailures);
        
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "test-request-id";

        // Act
        var (statusCode, response) = await handler.HandleAsync(exception, context);

        // Assert
        statusCode.Should().Be((int)HttpStatusCode.BadRequest);
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.Message.Should().Be("Validation failed");
        response.Meta.RequestId.Should().Be("test-request-id");
        response.Error.Should().NotBeNull();
        response.Error!.Type.Should().Be("ValidationError");
        response.Error.Details.Field.Should().Be("Name");
        response.Error.Details.Message.Should().Contain("Name is required");
        response.Error.Details.Message.Should().Contain("Invalid email format");
    }

    /// <summary>
    /// Tests that DefaultExceptionHandler can handle any exception type.
    /// Expects true for all exception types as it serves as a fallback handler.
    /// </summary>
    [Fact]
    public void DefaultExceptionHandler_CanHandle_AnyException_ReturnsTrue()
    {
        // Arrange
        var logger = new LoggerFactory().CreateLogger<DefaultExceptionHandler>();
        var handler = new DefaultExceptionHandler(logger);
        var exception = new InvalidOperationException("Test exception");

        // Act
        var canHandle = handler.CanHandle(exception);

        // Assert
        canHandle.Should().BeTrue();
    }

    /// <summary>
    /// Tests that DefaultExceptionHandler produces internal server error response.
    /// Expects InternalServerError status with generic error message for security.
    /// </summary>
    [Fact]
    public async Task DefaultExceptionHandler_HandleAsync_ReturnsInternalServerError()
    {
        // Arrange
        var logger = new LoggerFactory().CreateLogger<DefaultExceptionHandler>();
        var handler = new DefaultExceptionHandler(logger);
        var exception = new InvalidOperationException("Test exception");
        
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "test-request-id";

        // Act
        var (statusCode, response) = await handler.HandleAsync(exception, context);

        // Assert
        statusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.Message.Should().Be("An internal server error occurred");
        response.Meta.RequestId.Should().Be("test-request-id");
        response.Error.Should().NotBeNull();
        response.Error!.Type.Should().Be("InternalServerError");
    }

    /// <summary>
    /// Tests that exception handler chain can handle all exception types.
    /// Expects that every exception type finds an appropriate handler in the chain.
    /// </summary>
    [Fact]
    public void HandlerChain_WithDefaultHandler_AlwaysFindsHandler()
    {
        // Arrange
        var logger = new LoggerFactory().CreateLogger<DefaultExceptionHandler>();
        var handlers = new List<IExceptionHandler>
        {
            new ValidationExceptionHandler(),
            new DefaultExceptionHandler(logger) // DefaultExceptionHandler should be last
        };

        // Test with various exception types
        var exceptions = new Exception[]
        {
            new ValidationException(new List<ValidationFailure>()),
            new ArgumentException("Test"),
            new InvalidOperationException("Test"),
            new NotImplementedException("Test"),
            new Exception("Generic exception")
        };

        // Act & Assert
        foreach (var exception in exceptions)
        {
            var handler = handlers.FirstOrDefault(h => h.CanHandle(exception));
            handler.Should().NotBeNull($"No handler found for exception type {exception.GetType().Name}");
        }
    }
}