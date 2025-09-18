using CarLookup.Contracts.Dtos;
using CarLookup.Contracts.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Manager.Interfaces;

/// <summary>
/// Car model manager interface
/// </summary>
public interface ICarModelManager
{
    /// <summary>
    /// Get a specific car model by ID
    /// </summary>
    Task<CarModelDto> GetCarModelByIdAsync(Guid carModelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new car model
    /// </summary>
    Task<CarModelDto> CreateCarModelAsync(CarModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing car model
    /// </summary>
    Task<CarModelDto> UpdateCarModelAsync(Guid carModelId, CarModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a car model
    /// </summary>
    Task DeleteCarModelAsync(Guid carModelId, CancellationToken cancellationToken = default);
}
