using System;
namespace CarLookup.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationDomainException : DomainException
{
    public ValidationDomainException(string message) : base(message)
    {
    }

    public ValidationDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}