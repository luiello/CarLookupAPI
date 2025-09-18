using CarLookup.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CarLookup.Manager.Services.Dependencies;

/// <summary>
/// Infrastructure dependencies for car make operations
/// </summary>
public sealed class CarMakeInfra
{
    public ILogger Logger { get; }
    public IPaginationService PaginationService { get; }

    public CarMakeInfra(
        ILogger<CarMakeInfra> logger,
        IPaginationService paginationService)
    {
        Logger = logger;
        PaginationService = paginationService;
    }
}