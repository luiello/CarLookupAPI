namespace CarLookup.Contracts.Responses;

/// <summary>
/// Error details for failed responses
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Error type categorization
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information
    /// </summary>
    public ErrorDetailInfo Details { get; set; } = new();
}