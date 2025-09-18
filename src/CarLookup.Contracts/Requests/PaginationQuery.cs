namespace CarLookup.Contracts.Requests;

/// <summary>
/// Pagination query parameters
/// </summary>
public class PaginationQuery
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int Limit { get; set; } = 20;

    /// <summary>
    /// Filter by name containing this value
    /// </summary>
    public string NameContains { get; set; }

    public override string ToString()
    {
        return $"PageNumber={Page}, PageSize={Limit}, NameContains={NameContains}";
    }
}