using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing transactions across repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Get the make repository
    /// </summary>
    ICarMakeRepository MakeRepository { get; }

    /// <summary>
    /// Get the model repository
    /// </summary>
    ICarModelRepository ModelRepository { get; }

    /// <summary>
    /// Get the user repository
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Save all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected entities</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute a function within a transaction scope with automatic rollback on exceptions
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="operation">Operation to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute an action within a transaction scope with automatic rollback on exceptions
    /// </summary>
    /// <param name="operation">Operation to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}