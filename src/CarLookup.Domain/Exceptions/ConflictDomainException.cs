using System;
namespace CarLookup.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule conflict occurs
/// </summary>
public class ConflictDomainException : DomainException
{
    public ConflictDomainException(string message) : base(message)
    {
    }

    public ConflictDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}