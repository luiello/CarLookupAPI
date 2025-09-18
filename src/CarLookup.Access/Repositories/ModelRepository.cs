using CarLookup.Access.Extensions;
using CarLookup.Domain.Entities;
using CarLookup.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Access.Repositories;

/// <summary>
/// Repository implementation for car model operations
/// </summary>
public class ModelRepository : ICarModelRepository
{
    private readonly CarLookupDbContext _context;
    private readonly ILogger<ModelRepository> _logger;

    public ModelRepository(CarLookupDbContext context, ILogger<ModelRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(IEnumerable<CarModel> Items, long TotalCount)> GetCarModelsByMakeIdAsync(
        Guid makeId,
        int pageNumber,
        int pageSize,
        string nameContains = null,
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Getting car models for car make {MakeId}: page {PageNumber}, size {PageSize}, name filter: {NameFilter}, year filter: {YearFilter}", 
            makeId, pageNumber, pageSize, nameContains, year);

        var query = BuildBaseQuery(makeId, nameContains, year);

        var totalCount = await query.LongCountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return (Array.Empty<CarModel>(), 0);
        }

        var items = await query
            .OrderBy(cm => cm.Name)
            .ThenBy(cm => cm.ModelYear)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private IQueryable<CarModel> BuildBaseQuery(Guid carMakeId, string nameContains, int? year)
    {
        var query = _context.CarModels
            .AsNoTracking()
            .Where(cm => cm.MakeId == carMakeId);

        if (!string.IsNullOrWhiteSpace(nameContains))
        {
            query = query.Where(cm => EF.Functions.Like(cm.Name, nameContains.ToContainsPattern()));
        }

        if (year.HasValue)
        {
            query = query.Where(cm => cm.ModelYear == year.Value);
        }

        return query;
    }

    public async Task<CarModel> GetCarModelByIdAsync(Guid carModelId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting car model by ID: {ModelId}", carModelId);

        return await 
            _context.CarModels
            .AsNoTracking()
            .FirstOrDefaultAsync(cm => cm.ModelId == carModelId, cancellationToken);
    }

    public async Task<bool> ExistsByNameMakeAndYearAsync(
        string nameContains, 
        Guid makeId, 
        int year, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Checking if car model exists: {Name}, car make: {MakeId}, year: {Year}, excluding: {ExcludeId}", 
            nameContains, makeId, year, excludeId);

        var query = _context.CarModels
            .AsNoTracking()
            .Where(cm => 
                EF.Functions.Like(cm.Name, nameContains.ToContainsPattern())
                && cm.MakeId == makeId
                && cm.ModelYear == year);

        if (excludeId.HasValue)
        {
            query = query.Where(cm => cm.ModelId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CarModel> CreateCarModelAsync(CarModel carModel, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding car model to context: {Name} for car make: {MakeId}", carModel.Name, carModel.MakeId);

        _context.CarModels.Add(carModel);

        return await Task.FromResult(carModel);
    }

    public async Task<CarModel> UpdateCarModelAsync(CarModel carModel, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating car model in context: {ModelId}", carModel.ModelId);

        _context.CarModels.Update(carModel);

        return await Task.FromResult(carModel);
    }

    public async Task DeleteCarModelAsync(Guid carModelId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Marking car model for deletion: {ModelId}", carModelId);

        var carModel = await _context.CarModels
            .FindAsync([carModelId], cancellationToken);

        if (carModel != null)
        {
            _context.CarModels.Remove(carModel);
        }
    }

    public async Task<bool> HasCarModelsForMakeAsync(Guid carMakeId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if car make has car models: {MakeId}", carMakeId);

        return await _context.CarModels
            .AsNoTracking()
            .AnyAsync(cm => cm.MakeId == carMakeId, cancellationToken);
    }
}