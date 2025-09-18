using CarLookup.Contracts.Dtos;
using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using CarLookup.Domain.Authorization;
using CarLookup.Manager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Host.Controllers;

/// <summary>
/// Car models management controller
/// </summary>
[Route("api/v1/carmodels")]
[Tags("Car Models")]
public class CarModelsController : BaseApiController
{
    private readonly ICarModelManager _carModelManager;
    private readonly ILogger<CarModelsController> _logger;

    public CarModelsController(ICarModelManager carModelManager, ILogger<CarModelsController> logger)
    {
        _carModelManager = carModelManager;
        _logger = logger;
    }

    /// <summary>
    /// Get a specific car model by ID
    /// </summary>
    /// <param name="carModelId">Car model identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Car model details</returns>
    [HttpGet("{carModelId:guid}")]
    [Authorize(Policy = Policies.ReaderOrAbove)]
    public async Task<ActionResult<ApiResponse<CarModelDto>>> GetCarModelByIdAsync(
        Guid carModelId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting car model by ID: {CarModelId}", carModelId);

        var result = await _carModelManager.GetCarModelByIdAsync(carModelId, cancellationToken);

        return SuccessResponse(result, "Car model retrieved successfully");
    }

    /// <summary>
    /// Create a new car model
    /// </summary>
    /// <param name="request">Car model creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created car model</returns>
    [HttpPost]
    [Authorize(Policy = Policies.EditorOrAbove)]
    public async Task<ActionResult<ApiResponse<CarModelDto>>> CreateCarModelAsync(
        [FromBody] CarModelRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new car model: {Name} for car make: {MakeId}", request.Name, request.MakeId);

        var result = await _carModelManager.CreateCarModelAsync(request, cancellationToken);

        return CreatedResponse(
            result,
            nameof(GetCarModelByIdAsync),
            new { carModelId = result.ModelId },
            "Car model created successfully");
    }

    /// <summary>
    /// Update an existing car model
    /// </summary>
    /// <param name="carModelId">Car model identifier</param>
    /// <param name="request">Car model update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated car model</returns>
    [HttpPut("{carModelId:guid}")]
    [Authorize(Policy = Policies.EditorOrAbove)]
    public async Task<ActionResult<ApiResponse<CarModelDto>>> UpdateCarModelAsync(
        Guid carModelId,
        [FromBody] CarModelRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating car model: {CarModelId}", carModelId);

        var result = await _carModelManager.UpdateCarModelAsync(carModelId, request, cancellationToken);

        return SuccessResponse(result, "Car model updated successfully");
    }

    /// <summary>
    /// Delete a car model
    /// </summary>
    /// <param name="carModelId">Car model identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{carModelId:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCarModelAsync(
        Guid carModelId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting car model: {CarModelId}", carModelId);

        await _carModelManager.DeleteCarModelAsync(carModelId, cancellationToken);

        return SuccessResponse("Car model deleted successfully");
    }
}