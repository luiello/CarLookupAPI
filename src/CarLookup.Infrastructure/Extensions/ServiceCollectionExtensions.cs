using CarLookup.Access;
using CarLookup.Access.Repositories;
using CarLookup.Access.Seed;
using CarLookup.Domain.Authorization;
using CarLookup.Domain.Interfaces;
using CarLookup.Infrastructure.Authentication;
using CarLookup.Infrastructure.Middleware;
using CarLookup.Infrastructure.Middleware.Handlers;
using CarLookup.Infrastructure.Options;
using CarLookup.Infrastructure.Services;
using CarLookup.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CarLookup.Infrastructure.Extensions;

/// <summary>
/// Service configuration extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configure logging with Serilog
    /// </summary>
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("MachineName", Environment.MachineName)
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    /// <summary>
    /// Configure JWT authentication with structured error responses
    /// </summary>
    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = GetJwtOptions(builder.Configuration);

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => ConfigureJwtBearerOptions(options, jwtOptions));

        // Policy-based authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminOnly, policy =>
                policy.RequireRole(Roles.Admin));

            options.AddPolicy(Policies.EditorOrAbove, policy =>
                policy.RequireRole(Roles.Admin, Roles.Editor));

            options.AddPolicy(Policies.ReaderOrAbove, policy =>
                policy.RequireRole(Roles.Admin, Roles.Editor, Roles.Reader));
        });
        
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
    }

    /// <summary>
    /// Retrieves JWT options from configuration with validation
    /// </summary>
    private static JwtOptions GetJwtOptions(IConfiguration configuration)
    {
        return configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing or invalid");
    }

    /// <summary>
    /// Configures JWT Bearer options using the factory pattern
    /// </summary>
    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, JwtOptions jwtOptions)
    {
        var configuredOptions = JwtBearerOptionsFactory.Create(jwtOptions);
        
        options.TokenValidationParameters = configuredOptions.TokenValidationParameters;
        options.Events = configuredOptions.Events;
    }

    /// <summary>
    /// Configure Swagger/OpenAPI
    /// </summary>
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CarLookup API",
                Version = "v1",
                Description = "A RESTful API for managing car makes and models",
                Contact = new OpenApiContact
                {
                    Name = "CarLookup API by luiello",
                    Url = new Uri("https://github.com/luiello")
                }
            });

            c.OperationFilter<Filters.CamelCaseParameterOperationFilter>();

            // Add JWT authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments
            var xmlPath = Path.Combine(AppContext.BaseDirectory, "CarLookup.Host.xml");
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
    }

    /// <summary>
    /// Configure database
    /// </summary>
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration
                   .GetConnectionString("Default")
                   ?? throw new InvalidOperationException("Default connection string is not configured");

        builder.Services.AddDbContext<CarLookupDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
            {
                // Retry strategy for MySQL
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                
                // Command timeout
                mySqlOptions.CommandTimeout(30);

                // MySQL-specific options for better performance and compatibility
                mySqlOptions.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
            });

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine);
            }
        });

        // Register Unit of Work with proper dependencies
        builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<CarLookupDbContext>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new UnitOfWork(context, loggerFactory);
        });
        
        builder.Services.AddScoped<ICarMakeRepository, CarMakeRepository>();
        builder.Services.AddScoped<ICarModelRepository, ModelRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
    }

    /// <summary>
    /// Configure exception handling
    /// </summary>
    public static void ConfigureExceptionHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IExceptionHandler, ValidationExceptionHandler>();
        builder.Services.AddScoped<IExceptionHandler, NotFoundExceptionHandler>();
        builder.Services.AddScoped<IExceptionHandler, ConflictExceptionHandler>();
        builder.Services.AddScoped<IExceptionHandler, UnauthorizedAccessExceptionHandler>();
        builder.Services.AddScoped<IExceptionHandler, DefaultExceptionHandler>();
    }

    /// <summary>
    /// Configure application services
    /// </summary>
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Configure pagination options (centralized configuration)
        builder.Services.Configure<PaginationOptions>(builder.Configuration.GetSection(PaginationOptions.SectionName));

        // Infrastructure services
        builder.Services.AddScoped<IPaginationService, PaginationService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();

        // Controllers
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
        });
    }

    /// <summary>
    /// Configure CORS
    /// </summary>
    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // Allowing "Any" * for demo purposes
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders("X-Request-Id");
            });
        });
    }
}

/// <summary>
/// Application pipeline configuration extensions
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configure the application pipeline
    /// </summary>
    public static void ConfigurePipeline(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
            };
        });

        // Exception handling should be early in the pipeline
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarLookup API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Ensure database is created and seeded in development
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CarLookupDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            context.Database.EnsureCreated();

            if (configuration.GetValue<bool>("Data:SeedOnStartup", true))
            {
                SeedDatabase(context);
            }
        }
    }

    /// <summary>
    /// Seed the database with initial car makes and car models if they don't exist
    /// </summary>
    private static void SeedDatabase(CarLookupDbContext context)
    {
        try
        {
            // Seed car makes only if none exist
            if (!context.CarMakes.Any())
            {
                var carMakes = SeedData.GetCarMakes();
                context.CarMakes.AddRange(carMakes);
                context.SaveChanges();
                
                var logger = Log.ForContext<CarLookupDbContext>();
                logger.Information("Seeded {Count} car makes", carMakes.Count());
            }

            // Seed car models only if none exist
            if (!context.CarModels.Any())
            {
                var carModels = SeedData.GetCarModels();
                context.CarModels.AddRange(carModels);
                context.SaveChanges();
                
                var logger = Log.ForContext<CarLookupDbContext>();
                logger.Information("Seeded {Count} car models", carModels.Count());
            }
        }
        catch (Exception ex)
        {
            var logger = Log.ForContext<CarLookupDbContext>();
            logger.Error(ex, "An error occurred while seeding the database with car makes and car models");
            throw;
        }
    }
}