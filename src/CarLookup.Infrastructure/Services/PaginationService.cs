using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using CarLookup.Infrastructure.Options;
using CarLookup.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarLookup.Infrastructure.Services;

/// <summary>
/// Pagination service implementation
/// </summary>
public class PaginationService : IPaginationService
{
    private readonly PaginationOptions _options;

    public PaginationService(IOptions<PaginationOptions> options)
    {
        _options = options.Value;
    }

    public PaginationQuery Clamp(PaginationQuery query)
    {
        return new PaginationQuery
        {
            Page = Math.Max(1, query.Page),
            Limit = Math.Min(_options.MaxPageSize, Math.Max(1, query.Limit == 0 ? _options.DefaultPageSize : query.Limit)),
            NameContains = query.NameContains
        };
    }

    public CarModelPaginationQuery Clamp(CarModelPaginationQuery query)
    {
        return new CarModelPaginationQuery
        {
            Page = Math.Max(1, query.Page),
            Limit = Math.Min(_options.MaxPageSize, Math.Max(1, query.Limit == 0 ? _options.DefaultPageSize : query.Limit)),
            NameContains = query.NameContains,
            Year = query.Year
        };
    }

    public PaginationInfo CreatePaginationInfo(
        int page,
        int limit,
        long totalItems,
        string basePath,
        IDictionary<string, object> extraQuery = null)
    {
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / limit);
        var currentPage = totalPages == 0 ? 1 : Math.Min(page, totalPages);
        var hasNext = totalPages > 0 && currentPage < totalPages;
        var hasPrev = currentPage > 1;

        return new PaginationInfo
        {
            CurrentPage = currentPage,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Limit = limit,
            NextPage = hasNext ? BuildUrl(basePath, currentPage + 1, limit, extraQuery) : null,
            PrevPage = hasPrev ? BuildUrl(basePath, currentPage - 1, limit, extraQuery) : null
        };
    }

    private static string BuildUrl(string basePath, int page, int limit, IDictionary<string, object> extraQuery)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["page"] = page.ToString(),
            ["limit"] = limit.ToString()
        };

        if (extraQuery != null)
        {
            foreach (var param in extraQuery.Where(kvp => kvp.Value != null && !string.IsNullOrWhiteSpace(kvp.Value.ToString())))
            {
                queryParams[param.Key] = param.Value.ToString();
            }
        }

        return QueryHelpers.AddQueryString(basePath, queryParams);
    }
}