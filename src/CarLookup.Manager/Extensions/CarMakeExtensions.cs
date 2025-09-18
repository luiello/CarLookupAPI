using CarLookup.Contracts.Requests;
using CarLookup.Domain.Entities;
using System;

namespace CarLookup.Manager.Extensions;

/// <summary>
/// Extension methods for CarMake entity operations
/// </summary>
public static class CarMakeExtensions
{
    /// <summary>
    /// Create CarMake entity from CarMakeRequest
    /// </summary>
    /// <param name="request">The create request containing new values</param>
    /// <returns>New CarMake entity</returns>
    public static CarMake CreateFrom(this CarMakeRequest request)
    {
        return new CarMake
        {
            MakeId = Guid.NewGuid(),
            Name = request.Name,
            CountryOfOrigin = request.CountryOfOrigin,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Update CarMake entity from CarMakeRequest
    /// </summary>
    /// <param name="carMake">The car make entity to update</param>
    /// <param name="request">The update request containing new values</param>
    public static void UpdateFrom(this CarMake carMake, CarMakeRequest request)
    {
        carMake.Name = request.Name;
        carMake.CountryOfOrigin = request.CountryOfOrigin;
        carMake.UpdatedAt = DateTime.UtcNow;
    }
}
