namespace CarLookup.Infrastructure.Options;

/// <summary>
/// Pagination configuration options
/// </summary>
public class PaginationOptions
{
    public const string SectionName = "Pagination";

    /// <summary>
    /// Default page size
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Maximum allowed page size
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// Maximum length for name filter strings
    /// </summary>
    public int MaxNameFilterLength { get; set; } = 100;
}