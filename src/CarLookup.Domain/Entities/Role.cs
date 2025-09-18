using System;
using System.Collections.Generic;

namespace CarLookup.Domain.Entities;

/// <summary>
/// Represents a role in the system
/// </summary>
public class Role
{
    /// <summary>
    /// Unique identifier for the role
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Name of the role (e.g., "admin", "editor", "reader")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this role can do
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Collection of users assigned to this role
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// Timestamp when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the role was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}