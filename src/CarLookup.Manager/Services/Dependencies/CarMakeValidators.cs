using CarLookup.Contracts.Requests;
using FluentValidation;

namespace CarLookup.Manager.Services.Dependencies;

/// <summary>
/// Validator bundle for car make and car model related operations
/// </summary>
public sealed class CarMakeValidators
{
    public IValidator<CarMakeRequest> CarMakeCreate { get; }
    public IValidator<CarMakeRequest> CarMakeUpdate { get; }
    public IValidator<PaginationQuery> Pagination { get; }
    public IValidator<CarModelPaginationQuery> CarModelPagination { get; }

    public CarMakeValidators(
        IValidator<CarMakeRequest> carMakeCreate,
        IValidator<CarMakeRequest> carMakeUpdate,
        IValidator<PaginationQuery> pagination,
        IValidator<CarModelPaginationQuery> carModelPagination)
    {
        CarMakeCreate = carMakeCreate;
        CarMakeUpdate = carMakeUpdate;
        Pagination = pagination;
        CarModelPagination = carModelPagination;
    }
}