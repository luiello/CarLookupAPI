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
/// Repository implementation for car make operations
/// </summary>
public class CarMakeRepository : ICarMakeRepository
{
    private readonly CarLookupDbContext _context;
    private readonly ILogger<CarMakeRepository> _logger;

    public CarMakeRepository(CarLookupDbContext context, ILogger<CarMakeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(IEnumerable<CarMake> Items, long TotalCount)> GetCarMakesAsync(
        int pageNumber, 
        int pageSize, 
        string nameContains = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug( "Getting car makes: page {PageNumber}, size {PageSize}, filter: {Filter}", pageNumber, pageSize, nameContains);

        var query = _context.CarMakes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameContains))
        {
            query = query.Where(cm => EF.Functions.Like(cm.Name, nameContains.ToContainsPattern()));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderBy(cm => cm.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<CarMake> GetCarMakeByIdAsync(Guid carMakeId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting car make by ID: {MakeId}", carMakeId);

        return await _context.CarMakes
            .AsNoTracking()
            .FirstOrDefaultAsync(cm => cm.MakeId == carMakeId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string carMakeName, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if car make exists with name: {Name}, excluding: {ExcludeId}", carMakeName, excludeId);

        var query = _context.CarMakes
            .AsNoTracking()
            .Where(cm => EF.Functions.Like(cm.Name, carMakeName.ToContainsPattern()));

        if (excludeId.HasValue)
        {
            query = query.Where(cm => cm.MakeId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CarMake> CreateCarMakeAsync(CarMake carMake, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding car make to context: {Name}", carMake.Name);

        _context.CarMakes.Add(carMake);
        
        return await Task.FromResult(carMake);
    }

    public async Task<CarMake> UpdateMakeAsync(CarMake carMake, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating car make in context: {MakeId}", carMake.MakeId);

        _context.CarMakes.Update(carMake);
        
        return await Task.FromResult(carMake);
    }

    public async Task DeleteMakeAsync(Guid carMakeId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Marking car make for deletion: {MakeId}", carMakeId);

        var carMake = await _context.CarMakes.FindAsync([carMakeId], cancellationToken);
        if (carMake != null)
        {
            _context.CarMakes.Remove(carMake);
        }
    }
}