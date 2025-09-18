using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using System.Collections.Generic;

namespace CarLookup.Infrastructure.Services.Interfaces;

/// <summary>
/// Pagination service interface
/// </summary>
public interface IPaginationService
{
    /// <summary>
    /// Clamp pagination query to valid ranges
    /// </summary>
    PaginationQuery Clamp(PaginationQuery query);

    /// <summary>
    /// Clamp car model pagination query to valid ranges
    /// </summary>
    CarModelPaginationQuery Clamp(CarModelPaginationQuery query);

    /// <summary>
    /// Create pagination information for responses
    /// </summary>
    PaginationInfo CreatePaginationInfo(
        int page,
        int limit,
        long totalItems,
        string basePath,
        IDictionary<string, object> extraQuery = null);
}