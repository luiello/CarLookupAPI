using CarLookup.Infrastructure.Extensions;
using CarLookup.Manager.Interfaces;
using CarLookup.Manager.Services;
using CarLookup.Manager.Services.Dependencies;
using CarLookup.Manager.Validators;
using CarLookup.Infrastructure.Options;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CarLookup.Host;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services
        builder.ConfigureLogging();
        builder.ConfigureAuthentication();
        builder.ConfigureSwagger();
        builder.ConfigureDatabase();
        builder.ConfigureExceptionHandling();
        builder.ConfigureServices();
        builder.ConfigureCors();

        // Register grouped dependencies
        builder.Services.AddScoped<CarMakeValidators>();
        builder.Services.AddScoped<CarMakeInfra>();

        // Register Managers
        builder.Services.AddScoped<IAuthManager, AuthManager>();
        builder.Services.AddScoped<ICarMakeManager, CarMakeManager>();
        builder.Services.AddScoped<ICarModelManager, CarModelManager>();

        // Register FluentValidation validators with proper DI for PaginationOptions
        builder.Services.AddScoped(sp => new PaginationQueryValidator(sp.GetRequiredService<IOptions<PaginationOptions>>()));
        builder.Services.AddScoped(sp => new CarModelPaginationQueryValidator());

        // Register other validators using standard FluentValidation registration
        builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        var app = builder.Build();

        // Configure pipeline
        app.ConfigurePipeline();

        await app.RunAsync();
    }
}