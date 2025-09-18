using System.Threading;
using System.Threading.Tasks;
using CarLookup.Domain.Entities;

namespace CarLookup.Domain.Interfaces;

/// <summary>
/// Repository interface for User entity operations (authentication only)
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Get user by username including their roles
    /// </summary>
    /// <param name="username">Username to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User with roles if found, null otherwise</returns>
    Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}