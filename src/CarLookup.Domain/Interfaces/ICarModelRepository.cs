using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CarLookup.Domain.Entities;

namespace CarLookup.Domain.Interfaces;

/// <summary>
/// Repository interface for car model operations
/// </summary>
public interface ICarModelRepository
{
    /// <summary>
    /// Get car models by car make ID with pagination and filtering
    /// </summary>
    Task<(IEnumerable<CarModel> Items, long TotalCount)> GetCarModelsByMakeIdAsync(
        Guid makeId,
        int pageNumber,
        int pageSize,
        string nameContains = null,
        int? year = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a car model by its ID
    /// </summary>
    Task<CarModel> GetCarModelByIdAsync(Guid carModelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a car model exists with the given name, car make, and year
    /// </summary>
    Task<bool> ExistsByNameMakeAndYearAsync(
        string carMakeName,
        Guid carMakeId,
        int year,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new car model
    /// </summary>
    Task<CarModel> CreateCarModelAsync(CarModel carModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing car model
    /// </summary>
    Task<CarModel> UpdateCarModelAsync(CarModel carModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a car model
    /// </summary>
    Task DeleteCarModelAsync(Guid carModelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if any car models exist for the given car make
    /// </summary>
    Task<bool> HasCarModelsForMakeAsync(Guid carMakeId, CancellationToken cancellationToken = default);
}