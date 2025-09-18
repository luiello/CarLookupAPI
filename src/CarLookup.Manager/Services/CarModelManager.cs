using CarLookup.Contracts.Dtos;
using CarLookup.Contracts.Requests;
using CarLookup.Domain.Entities;
using CarLookup.Domain.Exceptions;
using CarLookup.Domain.Interfaces;
using CarLookup.Manager.Extensions;
using CarLookup.Manager.Interfaces;
using CarLookup.Manager.Services.Dependencies;
using Facet.Extensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Manager.Services;

/// <summary>
/// Car model manager implementation
/// </summary>
public class CarModelManager : ICarModelManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CarMakeInfra _infra;
    private readonly IValidator<CarModelRequest> _createValidator;
    private readonly IValidator<CarModelRequest> _updateValidator;

    public CarModelManager(
        IUnitOfWork unitOfWork,
        CarMakeInfra infra,
        IValidator<CarModelRequest> createValidator,
        IValidator<CarModelRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _infra = infra;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Get a specific car model by ID
    /// </summary>
    public async Task<CarModelDto> GetCarModelByIdAsync(Guid carModelId, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Getting car model by ID: {CarModelId}", carModelId);

        var carModel = await _unitOfWork.ModelRepository.GetCarModelByIdAsync(carModelId, cancellationToken);
        if (carModel == null)
        {
            throw new NotFoundDomainException("CarModel", carModelId);
        }

        var carModelDto = carModel.ToFacet<CarModel, CarModelDto>();

        return carModelDto;
    }

    /// <summary>
    /// Create a new car model
    /// </summary>
    public async Task<CarModelDto> CreateCarModelAsync(CarModelRequest request, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Creating new car model: {Name} for car make: {MakeId}", request.Name, request.MakeId);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var carMakeExists = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(request.MakeId, cancellationToken);
            if (carMakeExists == null)
            {
                throw new NotFoundDomainException("CarMake", request.MakeId);
            }

            var carModelExists = await _unitOfWork.ModelRepository.ExistsByNameMakeAndYearAsync(
                request.Name,
                request.MakeId,
                request.ModelYear,
                cancellationToken: cancellationToken);

            if (carModelExists)
            {
                throw new ConflictDomainException(
                    $"A car model with the name '{request.Name}' for year {request.ModelYear} already exists for this car make.");
            }

            var carModel = request.CreateFrom();

            var createdCarModel = await _unitOfWork.ModelRepository.CreateCarModelAsync(carModel, cancellationToken);

            var carModelDto = createdCarModel.ToFacet<CarModel, CarModelDto>();

            _infra.Logger.LogInformation("Created car model: {CarModelId} - {Name}", createdCarModel.ModelId, createdCarModel.Name);

            return carModelDto;

        }, cancellationToken);
    }

    /// <summary>
    /// Update an existing car model
    /// </summary>
    public async Task<CarModelDto> UpdateCarModelAsync(Guid carModelId, CarModelRequest request, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Updating car model: {CarModelId}", carModelId);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingCarModel = await _unitOfWork.ModelRepository.GetCarModelByIdAsync(carModelId, cancellationToken);
            if (existingCarModel == null)
            {
                throw new NotFoundDomainException("CarModel", carModelId);
            }

            if (existingCarModel.MakeId != request.MakeId)
            {
                var carMakeExists = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(request.MakeId, cancellationToken);
                if (carMakeExists == null)
                {
                    throw new NotFoundDomainException("CarMake", request.MakeId);
                }
            }

            var carModelExists = await _unitOfWork.ModelRepository.ExistsByNameMakeAndYearAsync(
                request.Name,
                request.MakeId,
                request.ModelYear,
                carModelId,
                cancellationToken);

            if (carModelExists)
            {
                throw new ConflictDomainException($"A car model with the name '{request.Name}' for year {request.ModelYear} already exists for this car make.");
            }

            existingCarModel.UpdateFrom(request);

            var updatedCarModel = await _unitOfWork.ModelRepository.UpdateCarModelAsync(existingCarModel, cancellationToken);

            var carModelDto = updatedCarModel.ToFacet<CarModel, CarModelDto>();

            _infra.Logger.LogInformation("Updated car model: {CarModelId} - {Name}", updatedCarModel.ModelId, updatedCarModel.Name);

            return carModelDto;

        }, cancellationToken);
    }

    /// <summary>
    /// Delete a car model
    /// </summary>
    public async Task DeleteCarModelAsync(Guid carModelId, CancellationToken cancellationToken = default)
    {
        _infra.Logger.LogInformation("Deleting car model: {CarModelId}", carModelId);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var carModel = await _unitOfWork.ModelRepository.GetCarModelByIdAsync(carModelId, cancellationToken);
            if (carModel == null)
            {
                throw new NotFoundDomainException("CarModel", carModelId);
            }

            await _unitOfWork.ModelRepository.DeleteCarModelAsync(carModelId, cancellationToken);

            _infra.Logger.LogInformation("Deleted car model: {CarModelId}", carModelId);

        }, cancellationToken);
    }
}