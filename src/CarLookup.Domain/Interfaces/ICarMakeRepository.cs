using CarLookup.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Domain.Interfaces;

/// <summary>
/// Repository interface for car make operations
/// </summary>
public interface ICarMakeRepository
{
    /// <summary>
    /// Get car makes with pagination and filtering
    /// </summary>
    Task<(IEnumerable<CarMake> Items, long TotalCount)> GetCarMakesAsync(
        int pageNumber,
        int pageSize,
        string nameContains = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a car make by its ID
    /// </summary>
    Task<CarMake> GetCarMakeByIdAsync(Guid carMakeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a car make exists with the given carMakeName
    /// </summary>
    Task<bool> ExistsByNameAsync(string carMakeName, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new car make
    /// </summary>
    Task<CarMake> CreateCarMakeAsync(CarMake carMake, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing car make
    /// </summary>
    Task<CarMake> UpdateMakeAsync(CarMake carMake, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a car make
    /// </summary>
    Task DeleteMakeAsync(Guid carMakeId, CancellationToken cancellationToken = default);
}