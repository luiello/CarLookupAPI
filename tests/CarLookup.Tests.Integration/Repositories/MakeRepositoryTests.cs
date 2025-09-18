using CarLookup.Access;
using CarLookup.Access.Repositories;
using CarLookup.TestData.Fakers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CarLookup.Tests.Integration.Repositories;

/// <summary>
/// Integration tests for CarMakeRepository data access operations.
/// Tests CRUD operations against an in-memory database to verify repository functionality.
/// </summary>
public class MakeRepositoryTests : IDisposable
{
    private readonly CarLookupDbContext _context;
    private readonly CarMakeRepository _repository;

    public MakeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CarLookupDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var logger = Mock.Of<ILogger<CarLookupDbContext>>();
        _context = new CarLookupDbContext(options, logger);

        var repositoryLogger = Mock.Of<ILogger<CarMakeRepository>>();
        _repository = new CarMakeRepository(_context, repositoryLogger);
    }

    /// <summary>
    /// Tests that GetCarMakesAsync returns paginated results with correct count.
    /// Expects specified number of items returned and accurate total count.
    /// </summary>
    [Fact]
    public async Task GetMakesAsync_WithValidParameters_ShouldReturnPaginatedCarMakes()
    {
        // Arrange
        var carMakes = CarMakeFaker.Generate(5);
        await _context.CarMakes.AddRangeAsync(carMakes);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetCarMakesAsync(1, 3);

        // Assert
        items.Should().HaveCount(3);
        totalCount.Should().Be(5);
    }

    /// <summary>
    /// Tests that GetCarMakeByIdAsync returns correct car make when given valid ID.
    /// Expects the retrieved car make to match the original data.
    /// </summary>
    [Fact]
    public async Task GetMakeByIdAsync_WithValidId_ShouldReturnCarMake()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCarMakeByIdAsync(carMake.MakeId);

        // Assert
        result.Should().NotBeNull();
        result!.MakeId.Should().Be(carMake.MakeId);
        result.Name.Should().Be(carMake.Name);
    }

    /// <summary>
    /// Tests that GetCarMakeByIdAsync returns null when given non-existent ID.
    /// Expects null result when no car make exists with the specified ID.
    /// </summary>
    [Fact]
    public async Task GetMakeByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.GetCarMakeByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that ExistsByNameAsync returns true when car make with given name exists.
    /// Expects true result when checking for existence of a car make by name.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_WithExistingCarMakeName_ShouldReturnTrue()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync(carMake.Name);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that CreateCarMakeAsync successfully creates and persists a new car make.
    /// Expects the car make to be created with correct data and persisted to database.
    /// </summary>
    [Fact]
    public async Task CreateMakeAsync_WithValidCarMake_ShouldCreateSuccessfully()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();

        // Act
        var result = await _repository.CreateCarMakeAsync(carMake);

        // Assert
        result.Should().NotBeNull();
        result.MakeId.Should().Be(carMake.MakeId);

        var savedCarMake = await _context.CarMakes.FindAsync(carMake.MakeId);
        savedCarMake.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that ExistsByNameAsync returns false when no car make exists with the given name.
    /// Expects false result when checking for non-existent car make name.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_WithNonExistentName_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentName = "NonExistentCarMake";

        // Act
        var result = await _repository.ExistsByNameAsync(nonExistentName);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ExistsByNameAsync properly excludes the specified ID when checking for duplicates.
    /// Expects false when checking name exists but excluding the same car make's ID.
    /// </summary>
    [Fact]
    public async Task ExistsByNameAsync_WithExcludeId_ShouldIgnoreSpecifiedMake()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();

        // Act - Check if name exists but exclude the same car make's ID
        var result = await _repository.ExistsByNameAsync(carMake.Name, carMake.MakeId);

        // Assert
        result.Should().BeFalse(); // Should return false because we're excluding this specific make
    }

    /// <summary>
    /// Tests that GetCarMakesAsync properly filters results by name when nameContains parameter is provided.
    /// Expects only car makes containing the search term to be returned.
    /// </summary>
    [Fact]
    public async Task GetMakesAsync_WithNameFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var toyotaMake = CarMakeFaker.Generate();
        toyotaMake.Name = "Lamborghini";
        
        var hondaMake = CarMakeFaker.Generate();
        hondaMake.Name = "Land Rover";
        
        var fordMake = CarMakeFaker.Generate();
        fordMake.Name = "Lexus";

        await _context.CarMakes.AddRangeAsync(toyotaMake, hondaMake, fordMake);
        await _context.SaveChangesAsync();

        // Act - should match Lamborghini and Land Rover
        var (items, totalCount) = await _repository.GetCarMakesAsync(1, 10, "La");

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(m => m.Name.Contains("La", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tests that GetCarMakesAsync handles pagination correctly across multiple pages.
    /// Expects correct items returned for different page numbers.
    /// </summary>
    [Fact]
    public async Task GetMakesAsync_WithPagination_ShouldReturnCorrectPages()
    {
        // Arrange
        var carMakes = CarMakeFaker.Generate(7); // Create 7 car makes
        await _context.CarMakes.AddRangeAsync(carMakes);
        await _context.SaveChangesAsync();

        // Act - Get first page (3 items)
        var (firstPageItems, firstPageTotal) = await _repository.GetCarMakesAsync(1, 3);
        
        // Act - Get second page (3 items)
        var (secondPageItems, secondPageTotal) = await _repository.GetCarMakesAsync(2, 3);
        
        // Act - Get third page (1 item)
        var (thirdPageItems, thirdPageTotal) = await _repository.GetCarMakesAsync(3, 3);

        // Assert
        firstPageItems.Should().HaveCount(3);
        firstPageTotal.Should().Be(7);
        
        secondPageItems.Should().HaveCount(3);
        secondPageTotal.Should().Be(7);
        
        thirdPageItems.Should().HaveCount(1);
        thirdPageTotal.Should().Be(7);

        // Ensure no duplicate items across pages
        var allItems = firstPageItems.Concat(secondPageItems).Concat(thirdPageItems);
        allItems.Select(m => m.MakeId).Should().OnlyHaveUniqueItems();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}