namespace CarLookup.Contracts.Responses;

/// <summary>
/// Pagination information for paginated responses
/// </summary>
public class PaginationInfo
{
    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public long TotalItems { get; set; }

    /// <summary>
    /// Number of items per page (limit)
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Link to next page (null if no next page)
    /// </summary>
    public string NextPage { get; set; }

    /// <summary>
    /// Link to previous page (null if no previous page)
    /// </summary>
    public string PrevPage { get; set; }
}