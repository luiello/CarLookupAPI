using Bogus;
using CarLookup.Contracts.Requests;
using System;

namespace CarLookup.TestData.Fakers;

/// <summary>
/// Bogus faker for request DTOs
/// </summary>
public static class RequestFakers
{
    /// <summary>
    /// Create a faker for CarMakeRequest
    /// </summary>
    public static Faker<CarMakeRequest> CarMakeRequest()
    {
        return new Faker<CarMakeRequest>()
            .RuleFor(r => r.Name, f => f.Vehicle.Manufacturer())
            .RuleFor(r => r.CountryOfOrigin, f => f.Address.Country());
    }

    /// <summary>
    /// Create a faker for CarModelRequest
    /// </summary>
    public static Faker<CarModelRequest> CarModelRequest(Guid? makeId = null)
    {
        return new Faker<CarModelRequest>()
            .RuleFor(r => r.MakeId, f => makeId ?? f.Random.Guid())
            .RuleFor(r => r.Name, f => f.Vehicle.Model())
            .RuleFor(r => r.ModelYear, f => f.Random.Int(1885, DateTime.UtcNow.Year + 1));
    }
}