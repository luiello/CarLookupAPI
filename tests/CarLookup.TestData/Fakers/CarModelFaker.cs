using Bogus;
using CarLookup.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CarLookup.TestData.Fakers;

/// <summary>
/// Bogus faker for CarModel entities
/// </summary>
public static class CarModelFaker
{
    /// <summary>
    /// Create a faker for CarModel entities
    /// </summary>
    public static Faker<CarModel> Create(Guid? makeId = null)
    {
        return new Faker<CarModel>()
            .RuleFor(m => m.ModelId, f => f.Random.Guid())
            .RuleFor(m => m.MakeId, f => makeId ?? f.Random.Guid())
            .RuleFor(m => m.Name, f => f.Vehicle.Model())
            .RuleFor(m => m.ModelYear, f => f.Random.Int(1885, DateTime.UtcNow.Year + 1))
            .RuleFor(m => m.CreatedAt, f => f.Date.PastOffset(1).DateTime)
            .RuleFor(m => m.UpdatedAt, f => f.Date.RecentOffset().DateTime);
    }

    /// <summary>
    /// Generate a single CarModel
    /// </summary>
    public static CarModel Generate(Guid? makeId = null)
    {
        return Create(makeId).Generate();
    }

    /// <summary>
    /// Generate multiple CarModels
    /// </summary>
    public static List<CarModel> Generate(int count, Guid? makeId = null)
    {
        return Create(makeId).Generate(count);
    }
}