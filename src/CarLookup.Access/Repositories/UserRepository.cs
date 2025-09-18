using CarLookup.Domain.Entities;
using CarLookup.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CarLookup.Access.Repositories;

/// <summary>
/// Repository implementation for User entity operations
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly CarLookupDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(CarLookupDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting user by username: {Username}", username);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive, cancellationToken);

        _logger.LogDebug("User {Username} found: {Found}", username, user != null);
       
        return user;
    }
}