using CarLookup.Contracts.Dtos;
using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Manager.Interfaces;

/// <summary>
/// Car make manager interface
/// </summary>
public interface ICarMakeManager
{
    /// <summary>
    /// Get all car makes with pagination and filtering
    /// </summary>
    Task<PagedResponse<CarMakeDto>> GetCarMakesAsync(PaginationQuery paginationQuery, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific car make by ID
    /// </summary>
    Task<CarMakeDto> GetCarMakeByIdAsync(Guid carMakeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new car make
    /// </summary>
    Task<CarMakeDto> CreateCarMakeAsync(CarMakeRequest carMakeCreateRequest, CancellationToken cancellationToken = default);

    /// <summary>  
    /// Update an existing car make
    /// </summary>
    Task<CarMakeDto> UpdateCarMakeAsync(Guid carMakeId, CarMakeRequest CarMakeRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a car make
    /// </summary>
    Task DeleteCarMakeAsync(Guid carMakeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get car models for a specific car make
    /// </summary>
    Task<PagedResponse<CarModelDto>> GetCarModelsByCarMakeIdAsync(Guid carMakeId, CarModelPaginationQuery carModelPaginationQuery, CancellationToken cancellationToken = default);
}
