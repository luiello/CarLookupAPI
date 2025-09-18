using System;
using System.Collections.Generic;

namespace CarLookup.Domain.Entities;

/// <summary>
/// Represents a car manufacturer/brand
/// </summary>
public class CarMake
{
    /// <summary>
    /// Unique identifier for the car make
    /// </summary>
    public Guid MakeId { get; set; }

    /// <summary>
    /// Name of the car make (e.g., Toyota, Honda)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Country where the brand is headquartered
    /// </summary>
    public string CountryOfOrigin { get; set; } = string.Empty;

    /// <summary>
    /// Collection of models associated with this make
    /// </summary>
    public virtual ICollection<CarModel> Models { get; set; } = [];

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}