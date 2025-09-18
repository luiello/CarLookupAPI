using CarLookup.Contracts.Requests;
using FluentValidation;
using Microsoft.Extensions.Options;
using CarLookup.Infrastructure.Options;

namespace CarLookup.Manager.Validators;

/// <summary>
/// Validator for pagination queries
/// </summary>
public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
{
    public PaginationQueryValidator(IOptions<PaginationOptions> options)
    {
        var paginationOptions = options.Value;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be 1 or greater");

        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be 1 or greater")
            .LessThanOrEqualTo(paginationOptions.MaxPageSize)
            .WithMessage($"Page size cannot be greater than {paginationOptions.MaxPageSize}");

        RuleFor(x => x.NameContains)
            .MaximumLength(paginationOptions.MaxNameFilterLength)
            .WithMessage($"Name filter cannot be longer than {paginationOptions.MaxNameFilterLength} characters")
            .When(x => !string.IsNullOrEmpty(x.NameContains));
    }
}