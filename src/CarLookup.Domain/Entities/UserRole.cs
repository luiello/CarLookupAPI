using System;

namespace CarLookup.Domain.Entities;

/// <summary>
/// Junction entity for many-to-many relationship between Users and Roles
/// </summary>
public class UserRole
{
    /// <summary>
    /// Foreign key to the User
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the User
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Foreign key to the Role
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Navigation property to the Role
    /// </summary>
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Timestamp when the user was assigned this role
    /// </summary>
    public DateTime AssignedAt { get; set; }
}