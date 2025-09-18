using CarLookup.Contracts.Requests;
using FluentValidation;
using System;

namespace CarLookup.Manager.Validators;

/// <summary>
/// Validator for car model pagination query
/// </summary>
public class CarModelPaginationQueryValidator : AbstractValidator<CarModelPaginationQuery>
{
    public CarModelPaginationQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.NameContains)
            .MaximumLength(50)
            .WithMessage("Name filter cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.NameContains));

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(1885)
            .WithMessage("Year must be 1885 or later")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
            .WithMessage($"Year cannot be later than {DateTime.UtcNow.Year + 1}")
            .When(x => x.Year.HasValue);
    }
}