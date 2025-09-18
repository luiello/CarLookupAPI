using Bogus;
using CarLookup.Domain.Entities;
using System.Collections.Generic;

namespace CarLookup.TestData.Fakers;

/// <summary>
/// Bogus faker for CarMake entities
/// </summary>
public static class CarMakeFaker
{
    /// <summary>
    /// Create a faker for CarMake entities
    /// </summary>
    public static Faker<CarMake> Create()
    {
        return new Faker<CarMake>()
            .RuleFor(m => m.MakeId, f => f.Random.Guid())
            .RuleFor(m => m.Name, f => f.Vehicle.Manufacturer())
            .RuleFor(m => m.CountryOfOrigin, f => f.Address.Country())
            .RuleFor(m => m.CreatedAt, f => f.Date.PastOffset(1).DateTime)
            .RuleFor(m => m.UpdatedAt, f => f.Date.RecentOffset().DateTime)
            .RuleFor(m => m.Models, f => new List<CarModel>());
    }

    /// <summary>
    /// Generate a single CarMake
    /// </summary>
    public static CarMake Generate()
    {
        return Create().Generate();
    }

    /// <summary>
    /// Generate multiple CarMakes
    /// </summary>
    public static List<CarMake> Generate(int count)
    {
        return Create().Generate(count);
    }
}