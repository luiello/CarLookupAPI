namespace CarLookup.Contracts.Responses;

/// <summary>
/// Generic response envelope for API responses
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Human-readable message about the operation
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The response data
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Pagination information (only for paginated responses)
    /// </summary>
    public PaginationInfo Pagination { get; set; }

    /// <summary>
    /// Response metadata
    /// </summary>
    public ResponseMeta Meta { get; set; } = new();

    /// <summary>
    /// Error information (only when Success is false)
    /// </summary>
    public ErrorDetails Error { get; set; }
}