using CarLookup.Contracts.Requests;
using FluentValidation;

namespace CarLookup.Manager.Validators;

/// <summary>
/// Validator for car make update requests
/// </summary>
public class CarMakeUpdateRequestValidator : AbstractValidator<CarMakeRequest>
{
    public CarMakeUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.CountryOfOrigin)
            .NotEmpty()
            .WithMessage("Country of origin is required")
            .Length(2, 100)
            .WithMessage("Country of origin must be between 2 and 100 characters");
    }
}