namespace CarLookup.Contracts.Requests;

/// <summary>
/// Pagination query parameters for car models with additional filtering
/// </summary>
public class CarModelPaginationQuery : PaginationQuery
{
    /// <summary>
    /// Filter by model year
    /// </summary>
    public int? Year { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, Year={Year}";
    }
}