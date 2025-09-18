namespace CarLookup.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundDomainException : DomainException
{
    public NotFoundDomainException(string entityName, object id)
        : base($"{entityName} with ID '{id}' was not found.")
    {
    }

    public NotFoundDomainException(string message) : base(message)
    {
    }
}