using CarLookup.Contracts.Requests;
using FluentValidation;
using System;

namespace CarLookup.Manager.Validators;

/// <summary>
/// Validator for car model update requests
/// </summary>
public class CarModelUpdateRequestValidator : AbstractValidator<CarModelRequest>
{
    public CarModelUpdateRequestValidator()
    {
        RuleFor(x => x.MakeId)
            .NotEmpty()
            .WithMessage("Make ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(1, 120)
            .WithMessage("Name must be between 1 and 120 characters");

        RuleFor(x => x.ModelYear)
            .GreaterThanOrEqualTo(1885)
            .WithMessage("Model year must be 1885 or later")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
            .WithMessage($"Model year cannot be later than {DateTime.UtcNow.Year + 1}");
    }
}