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
/// Car makes management controller
/// </summary>
[Route("api/v1/carmakes")]
[Tags("Car Makes")]
public class CarMakesController : BaseApiController
{
    private readonly ICarMakeManager _carMakeManager;
    private readonly ILogger<CarMakesController> _logger;

    public CarMakesController(ICarMakeManager carMakeManager, ILogger<CarMakesController> logger)
    {
        _carMakeManager = carMakeManager;
        _logger = logger;
    }

    /// <summary>
    /// Get all car makes with pagination and filtering
    /// </summary>
    /// <param name="paginationQuery">Pagination query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of car makes</returns>
    [HttpGet]
    [Authorize(Policy = Policies.ReaderOrAbove)]
    public async Task<ActionResult<PagedResponse<CarMakeDto>>> GetCarMakesAsync(
        [FromQuery] PaginationQuery paginationQuery,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting car makes with pagination: {Query}", paginationQuery);

        var result = await _carMakeManager.GetCarMakesAsync(paginationQuery, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a specific car make by ID
    /// </summary>
    /// <param name="carMakeId">Car make identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Car make details</returns>
    [HttpGet("{carMakeId:guid}")]
    [Authorize(Policy = Policies.ReaderOrAbove)]
    public async Task<ActionResult<ApiResponse<CarMakeDto>>> GetCarMakeByIdAsync(
        Guid carMakeId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting car make by ID: {CarMakeId}", carMakeId);

        var result = await _carMakeManager.GetCarMakeByIdAsync(carMakeId, cancellationToken);

        return SuccessResponse(result, "Car make retrieved successfully");
    }

    /// <summary>
    /// Get car models for a specific car make
    /// </summary>
    /// <param name="carMakeId">Car make identifier</param>
    /// <param name="paginationQuery">Pagination query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of car models for the car make</returns>
    [HttpGet("{carMakeId:guid}/carmodels")]
    [Authorize(Policy = Policies.ReaderOrAbove)]
    public async Task<ActionResult<PagedResponse<CarModelDto>>> GetCarModelsByCarMakeIdAsync(
        Guid carMakeId,
        [FromQuery] CarModelPaginationQuery paginationQuery,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting car models for car make: {CarMakeId}", carMakeId);

        var result = await _carMakeManager.GetCarModelsByCarMakeIdAsync(carMakeId, paginationQuery, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create a new car make
    /// </summary>
    /// <param name="request">Car make creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created car make</returns>
    [HttpPost]
    [Authorize(Policy = Policies.EditorOrAbove)]
    public async Task<ActionResult<ApiResponse<CarMakeDto>>> CreateCarMakeAsync(
        [FromBody] CarMakeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new car make: {Name}", request.Name);

        var result = await _carMakeManager.CreateCarMakeAsync(request, cancellationToken);

        return CreatedResponse(
            result,
            nameof(GetCarMakeByIdAsync),
            new { carMakeId = result.MakeId },
            "Car make created successfully");
    }

    /// <summary>
    /// Update an existing car make
    /// </summary>
    /// <param name="carMakeId">Car make identifier</param>
    /// <param name="request">Car make update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated car make</returns>
    [HttpPut("{carMakeId:guid}")]
    [Authorize(Policy = Policies.EditorOrAbove)]
    public async Task<ActionResult<ApiResponse<CarMakeDto>>> UpdateCarMakeAsync(
        Guid carMakeId,
        [FromBody] CarMakeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating car make: {CarMakeId}", carMakeId);

        var result = await _carMakeManager.UpdateCarMakeAsync(carMakeId, request, cancellationToken);

        return SuccessResponse(result, "Car make updated successfully");
    }

    /// <summary>
    /// Delete a car make
    /// </summary>
    /// <param name="carMakeId">Car make identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{carMakeId:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCarMakeAsync(
        Guid carMakeId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting car make: {CarMakeId}", carMakeId);

        await _carMakeManager.DeleteCarMakeAsync(carMakeId, cancellationToken);

        return SuccessResponse("Car make deleted successfully");
    }
}