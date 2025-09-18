namespace CarLookup.Contracts.Responses;

/// <summary>
/// Detailed error information
/// </summary>
public class ErrorDetailInfo
{
    /// <summary>
    /// Field name associated with the error (for validation errors)
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Detailed error message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}