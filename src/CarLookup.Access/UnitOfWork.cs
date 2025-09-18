using CarLookup.Access.Repositories;
using CarLookup.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Access;

/// <summary>
/// Unit of Work implementation for managing transactions across repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CarLookupDbContext _context;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<UnitOfWork> _logger;
    private bool _disposed = false;

    // Lazy initialization of repositories
    private ICarMakeRepository _makeRepository;
    private ICarModelRepository _modelRepository;
    private IUserRepository _userRepository;

    public UnitOfWork(CarLookupDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<UnitOfWork>();
    }

    public ICarMakeRepository MakeRepository => _makeRepository ??= new CarMakeRepository(_context, _loggerFactory.CreateLogger<CarMakeRepository>());

    public ICarModelRepository ModelRepository => _modelRepository ??= new ModelRepository(_context, _loggerFactory.CreateLogger<ModelRepository>());

    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context, _loggerFactory.CreateLogger<UserRepository>());

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes to database");

        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Successfully saved {Count} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes to database");
            throw;
        }
    }

    public virtual async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting transactional operation with return value");

        // Checking if there is an open transaction already
        if (_context.Database.CurrentTransaction != null)
        {
            _logger.LogDebug("Already in a transaction, executing operation directly");
            return await operation();
        }

        // Use the Execution Strategy to handle retries properly with transactions
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            operation,
            async (context, op, ct) =>
            {
                using var transaction = await context.Database.BeginTransactionAsync(ct);
                try
                {
                    var result = await op();
                    await SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    _logger.LogDebug("Transaction completed successfully");

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Transaction failed, rolling back");
                    await transaction.RollbackAsync(ct);

                    throw;
                }
            },
            null,
            cancellationToken);
    }

    public virtual async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting transactional operation without return value");

        // Checking if there is an open transaction already
        if (_context.Database.CurrentTransaction != null)
        {
            _logger.LogDebug("Already in a transaction, executing operation directly");
            await operation();
            return;
        }

        // Use the Execution Strategy to handle retries properly with transactions
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync<Func<Task>, object>(
            operation,
            async (context, op, ct) =>
            {
                using var transaction = await context.Database.BeginTransactionAsync(ct);
                try
                {
                    await op();
                    await SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    _logger.LogDebug("Transaction completed successfully");
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Transaction failed, rolling back");
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            },
            null,
            cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}