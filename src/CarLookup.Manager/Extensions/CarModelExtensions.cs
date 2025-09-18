using CarLookup.Contracts.Requests;
using CarLookup.Domain.Entities;
using System;

namespace CarLookup.Manager.Extensions;

/// <summary>
/// Extension methods for CarModel entity operations
/// </summary>
public static class CarModelExtensions
{
    /// <summary>
    /// Create CarModel entity from CarModelRequest
    /// </summary>
    /// <param name="request">The create request containing new values</param>
    /// <returns>New CarModel entity</returns>
    public static CarModel CreateFrom(this CarModelRequest request)
    {
        return new CarModel
        {
            ModelId = Guid.NewGuid(),
            MakeId = request.MakeId,
            Name = request.Name,
            ModelYear = request.ModelYear,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Update CarModel entity from CarModelRequest
    /// </summary>
    /// <param name="carModel">The car model entity to update</param>
    /// <param name="request">The update request containing new values</param>
    public static void UpdateFrom(this CarModel carModel, CarModelRequest request)
    {
        carModel.MakeId = request.MakeId;
        carModel.Name = request.Name;
        carModel.ModelYear = request.ModelYear;
        carModel.UpdatedAt = DateTime.UtcNow;
    }
}
