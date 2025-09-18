using System;

namespace CarLookup.Domain.Entities;

/// <summary>
/// Represents a specific car model under a make
/// </summary>
public class CarModel
{
    /// <summary>
    /// Unique identifier for the car model
    /// </summary>
    public Guid ModelId { get; set; }

    /// <summary>
    /// Foreign key to the associated car make
    /// </summary>
    public Guid MakeId { get; set; }

    /// <summary>
    /// Name of the model (e.g., Camry, Accord)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Year of production for this model
    /// </summary>
    public int ModelYear { get; set; }

    /// <summary>
    /// Navigation property to the associated make
    /// </summary>
    public virtual CarMake Make { get; set; } = null!;

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}