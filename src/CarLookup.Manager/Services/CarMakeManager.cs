using CarLookup.Contracts.Dtos;
using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using CarLookup.Domain.Entities;
using CarLookup.Domain.Exceptions;
using CarLookup.Domain.Interfaces;
using CarLookup.Manager.Extensions;
using CarLookup.Manager.Interfaces;
using CarLookup.Manager.Services.Dependencies;
using Facet.Extensions;
using Facet.Mapping;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Manager.Services;

/// <summary>
/// Car make manager implementation
/// </summary>
public class CarMakeManager : ICarMakeManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CarMakeValidators _validators;
    private readonly CarMakeInfra _infra;

    public CarMakeManager(
        IUnitOfWork unitOfWork,
        CarMakeValidators validators,
        CarMakeInfra infra)
    {
        _unitOfWork = unitOfWork;
        _validators = validators;
        _infra = infra;
    }

    /// <summary>
    /// Get all car makes with pagination and filtering
    /// </summary>
    public async Task<PagedResponse<CarMakeDto>> GetCarMakesAsync(PaginationQuery paginationQuery, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Getting car makes with query: {Query}", paginationQuery);

        var validationResult = await _validators.Pagination.ValidateAsync(paginationQuery, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var clampedQuery = _infra.PaginationService.Clamp(paginationQuery);

        var (items, totalCount) = await _unitOfWork.MakeRepository.GetCarMakesAsync(
            clampedQuery.Page,
            clampedQuery.Limit,
            clampedQuery.NameContains,
            cancellationToken);

        var carMakeDtos = items.SelectFacets<CarMake, CarMakeDto>();

        var extraQuery = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(clampedQuery.NameContains))
        {
            extraQuery["nameContains"] = clampedQuery.NameContains;
        }

        var response = new PagedResponse<CarMakeDto>
        {
            Success = true,
            Message = "Successful",
            Data = carMakeDtos,
            Pagination = _infra.PaginationService.CreatePaginationInfo(
                clampedQuery.Page,
                clampedQuery.Limit,
                totalCount,
                "/api/v1/carmakes",
                extraQuery.Any() ? extraQuery : null),
            Meta = new ResponseMeta { RequestId = Guid.NewGuid().ToString(), Timestamp = DateTime.UtcNow }
        };

        _infra.Logger.LogInformation("Retrieved {Count} car makes (page {Page} of {TotalPages})",
            carMakeDtos.Count(), clampedQuery.Page, response.Pagination?.TotalPages);

        return response;
    }

    /// <summary>
    /// Get a specific car make by ID
    /// </summary>
    public async Task<CarMakeDto> GetCarMakeByIdAsync(Guid carMakeId, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Getting car make by ID: {CarMakeId}", carMakeId);

        var carMake = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(carMakeId, cancellationToken);
        if (carMake == null)
        {
            throw new NotFoundDomainException("CarMake", carMakeId);
        }

        var carMakeDto = carMake.ToFacet<CarMake, CarMakeDto>();

        return carMakeDto;
    }

    /// <summary>
    /// Create a new car make
    /// </summary>
    public async Task<CarMakeDto> CreateCarMakeAsync(CarMakeRequest carMakeCreateRequest, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Creating new car make: {Name}", carMakeCreateRequest.Name);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var validationResult = await _validators.CarMakeCreate.ValidateAsync(carMakeCreateRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var nameExists = await _unitOfWork.MakeRepository.ExistsByNameAsync(carMakeCreateRequest.Name, cancellationToken: cancellationToken);
            if (nameExists)
            {
                throw new ConflictDomainException($"A car make with the name '{carMakeCreateRequest.Name}' already exists.");
            }

            var carMake = carMakeCreateRequest.CreateFrom();

            var createdCarMake = await _unitOfWork.MakeRepository.CreateCarMakeAsync(carMake, cancellationToken);

            var carMakeDto = carMake.ToFacet<CarMake, CarMakeDto>();

            _infra.Logger.LogInformation("Created car make: {CarMakeId} - {Name}", createdCarMake.MakeId, createdCarMake.Name);

            return carMakeDto;

        }, cancellationToken);
    }

    /// <summary>
    /// Update an existing car make
    /// </summary>
    public async Task<CarMakeDto> UpdateCarMakeAsync(Guid carMakeId, CarMakeRequest carMakeUpdateRequest, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Updating car make: {CarMakeId}", carMakeId);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var validationResult = await _validators.CarMakeUpdate.ValidateAsync(carMakeUpdateRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingCarMake = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(carMakeId, cancellationToken);
            if (existingCarMake == null)
            {
                throw new NotFoundDomainException("CarMake", carMakeId);
            }

            var nameExists = await _unitOfWork.MakeRepository.ExistsByNameAsync(carMakeUpdateRequest.Name, carMakeId, cancellationToken);
            if (nameExists)
            {
                throw new ConflictDomainException($"A car make with the name '{carMakeUpdateRequest.Name}' already exists.");
            }

            existingCarMake.UpdateFrom(carMakeUpdateRequest);

            var updatedCarMake = await _unitOfWork.MakeRepository.UpdateMakeAsync(existingCarMake, cancellationToken);

            var carMakeDto = updatedCarMake.ToFacet<CarMake, CarMakeDto>();

            _infra.Logger.LogInformation("Updated car make: {CarMakeId} - {Name}", updatedCarMake.MakeId, updatedCarMake.Name);

            return carMakeDto;

        }, cancellationToken);
    }

    /// <summary>
    /// Delete a car make
    /// </summary>
    public async Task DeleteCarMakeAsync(Guid carMakeId, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Deleting car make: {CarMakeId}", carMakeId);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var carMake = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(carMakeId, cancellationToken);
            if (carMake == null)
            {
                throw new NotFoundDomainException("CarMake", carMakeId);
            }

            var hasCarModels = await _unitOfWork.ModelRepository.HasCarModelsForMakeAsync(carMakeId, cancellationToken);
            if (hasCarModels)
            {
                throw new ConflictDomainException("Cannot delete car make because it has associated car models. Delete the car models first.");
            }

            await _unitOfWork.MakeRepository.DeleteMakeAsync(carMakeId, cancellationToken);

            _infra.Logger.LogInformation("Deleted car make: {CarMakeId}", carMakeId);

        }, cancellationToken);
    }

    /// <summary>
    /// Get car models for a specific car make
    /// </summary>
    public async Task<PagedResponse<CarModelDto>> GetCarModelsByCarMakeIdAsync(Guid carMakeId, CarModelPaginationQuery carModelPaginationQuery, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation(
            "Getting car models for car make: {CarMakeId} with query: {Query}",
            carMakeId, carModelPaginationQuery);

        var validationResult = await _validators.CarModelPagination.ValidateAsync(carModelPaginationQuery, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var carMake = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(carMakeId, cancellationToken);
        if (carMake == null)
        {
            throw new NotFoundDomainException("CarMake", carMakeId);
        }

        var clampedQuery = _infra.PaginationService.Clamp(carModelPaginationQuery);

        var (items, totalCount) = await _unitOfWork.ModelRepository.GetCarModelsByMakeIdAsync(
            carMakeId,
            clampedQuery.Page,
            clampedQuery.Limit,
            clampedQuery.NameContains,
            clampedQuery.Year,
            cancellationToken);

        var carModelDtos = items.SelectFacets<CarModel, CarModelDto>();

        var extraQuery = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(clampedQuery.NameContains))
        {
            extraQuery["nameContains"] = clampedQuery.NameContains;
        }
        if (clampedQuery.Year.HasValue)
        {
            extraQuery["year"] = clampedQuery.Year.Value;
        }

        var response = new PagedResponse<CarModelDto>
        {
            Success = true,
            Message = "Successful",
            Data = carModelDtos,
            Pagination = _infra.PaginationService.CreatePaginationInfo(
                clampedQuery.Page,
                clampedQuery.Limit,
                totalCount,
                $"/api/v1/carmakes/{carMakeId}/carmodels",
                extraQuery.Any() ? extraQuery : null),
            Meta = new ResponseMeta
            {
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            }
        };

        _infra.Logger.LogInformation(
            "Retrieved {Count} car models for car make {CarMakeId} (page {Page} of {TotalPages})",
            carModelDtos.Count(), carMakeId, clampedQuery.Page, response.Pagination?.TotalPages);

        return response;
    }
}