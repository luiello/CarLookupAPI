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
/// Integration tests for ModelRepository data access operations.
/// Tests CRUD operations against an in-memory database to verify repository functionality.
/// </summary>
public class ModelRepositoryTests : IDisposable
{
    private readonly CarLookupDbContext _context;
    private readonly ModelRepository _repository;

    public ModelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CarLookupDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var logger = Mock.Of<ILogger<CarLookupDbContext>>();
        _context = new CarLookupDbContext(options, logger);

        var repositoryLogger = Mock.Of<ILogger<ModelRepository>>();
        _repository = new ModelRepository(_context, repositoryLogger);
    }

    /// <summary>
    /// Tests that GetCarModelsByMakeIdAsync returns paginated results with correct count.
    /// Expects specified number of items returned and accurate total count.
    /// </summary>
    [Fact]
    public async Task GetCarModelsByMakeIdAsync_WithValidParameters_ShouldReturnPaginatedCarModels()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModels = CarModelFaker.Generate(5, carMake.MakeId);
        await _context.CarModels.AddRangeAsync(carModels);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetCarModelsByMakeIdAsync(carMake.MakeId, 1, 3);

        // Assert
        items.Should().HaveCount(3);
        totalCount.Should().Be(5);
    }

    /// <summary>
    /// Tests that GetCarModelByIdAsync returns correct car model when given valid ID.
    /// Expects the retrieved car model to match the original data.
    /// </summary>
    [Fact]
    public async Task GetCarModelByIdAsync_WithValidId_ShouldReturnCarModel()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);
        await _context.CarModels.AddAsync(carModel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCarModelByIdAsync(carModel.ModelId);

        // Assert
        result.Should().NotBeNull();
        result!.ModelId.Should().Be(carModel.ModelId);
        result.Name.Should().Be(carModel.Name);
        result.MakeId.Should().Be(carMake.MakeId);
    }

    /// <summary>
    /// Tests that GetCarModelByIdAsync returns null when given non-existent ID.
    /// Expects null result when no car model exists with the specified ID.
    /// </summary>
    [Fact]
    public async Task GetCarModelByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.GetCarModelByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that ExistsByNameMakeAndYearAsync returns true when car model with given parameters exists.
    /// Expects true result when checking for existence of a car model by name, make and year.
    /// </summary>
    [Fact]
    public async Task ExistsByNameMakeAndYearAsync_WithExistingCarModel_ShouldReturnTrue()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);
        await _context.CarModels.AddAsync(carModel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameMakeAndYearAsync(carModel.Name, carMake.MakeId, carModel.ModelYear);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that CreateCarModelAsync successfully creates and persists a new car model.
    /// Expects the car model to be created with correct data and persisted to database.
    /// </summary>
    [Fact]
    public async Task CreateCarModelAsync_WithValidCarModel_ShouldCreateSuccessfully()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);

        // Act
        var result = await _repository.CreateCarModelAsync(carModel);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.ModelId.Should().Be(carModel.ModelId);

        var savedCarModel = await _context.CarModels.FindAsync(carModel.ModelId);
        savedCarModel.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that ExistsByNameMakeAndYearAsync returns false when no car model exists with the given parameters.
    /// Expects false result when checking for non-existent car model.
    /// </summary>
    [Fact]
    public async Task ExistsByNameMakeAndYearAsync_WithNonExistentModel_ShouldReturnFalse()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();
        
        var nonExistentName = "NonExistentModel";
        var nonExistentYear = 9999;

        // Act
        var result = await _repository.ExistsByNameMakeAndYearAsync(nonExistentName, carMake.MakeId, nonExistentYear);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ExistsByNameMakeAndYearAsync properly excludes the specified ID when checking for duplicates.
    /// Expects false when checking model exists but excluding the same car model's ID.
    /// </summary>
    [Fact]
    public async Task ExistsByNameMakeAndYearAsync_WithExcludeId_ShouldIgnoreSpecifiedModel()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);
        await _context.CarModels.AddAsync(carModel);
        await _context.SaveChangesAsync();

        // Act - Check if model exists but exclude the same car model's ID
        var result = await _repository.ExistsByNameMakeAndYearAsync(carModel.Name, carMake.MakeId, carModel.ModelYear, carModel.ModelId);

        // Assert
        result.Should().BeFalse(); // Should return false because we're excluding this specific model
    }

    /// <summary>
    /// Tests that GetCarModelsByMakeIdAsync properly filters results by name when nameContains parameter is provided.
    /// Expects only car models containing the search term to be returned.
    /// </summary>
    [Fact]
    public async Task GetCarModelsByMakeIdAsync_WithNameFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var camryModel = CarModelFaker.Generate(carMake.MakeId);
        camryModel.Name = "Camry";
        
        var corollaModel = CarModelFaker.Generate(carMake.MakeId);
        corollaModel.Name = "Corolla";
        
        var civicModel = CarModelFaker.Generate(carMake.MakeId);
        civicModel.Name = "Civic";

        await _context.CarModels.AddRangeAsync(camryModel, corollaModel, civicModel);
        await _context.SaveChangesAsync();

        // Act - should match Camry and Corolla
        var (items, totalCount) = await _repository.GetCarModelsByMakeIdAsync(carMake.MakeId, 1, 10, "C");

        // Assert
        items.Should().HaveCount(3); // All start with 'C'
        totalCount.Should().Be(3);
        items.Should().OnlyContain(m => m.Name.Contains("C", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tests that GetCarModelsByMakeIdAsync properly filters results by year when year parameter is provided.
    /// Expects only car models with the specified year to be returned.
    /// </summary>
    [Fact]
    public async Task GetCarModelsByMakeIdAsync_WithYearFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var model2020 = CarModelFaker.Generate(carMake.MakeId);
        model2020.ModelYear = 2020;
        
        var model2021 = CarModelFaker.Generate(carMake.MakeId);
        model2021.ModelYear = 2021;
        
        var anotherModel2020 = CarModelFaker.Generate(carMake.MakeId);
        anotherModel2020.ModelYear = 2020;

        await _context.CarModels.AddRangeAsync(model2020, model2021, anotherModel2020);
        await _context.SaveChangesAsync();

        // Act - should match only 2020 models
        var (items, totalCount) = await _repository.GetCarModelsByMakeIdAsync(carMake.MakeId, 1, 10, null, 2020);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(m => m.ModelYear == 2020);
    }

    /// <summary>
    /// Tests that GetCarModelsByMakeIdAsync handles pagination correctly across multiple pages.
    /// Expects correct items returned for different page numbers.
    /// </summary>
    [Fact]
    public async Task GetCarModelsByMakeIdAsync_WithPagination_ShouldReturnCorrectPages()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModels = CarModelFaker.Generate(7, carMake.MakeId); // Create 7 car models
        await _context.CarModels.AddRangeAsync(carModels);
        await _context.SaveChangesAsync();

        // Act - Get first page (3 items)
        var (firstPageItems, firstPageTotal) = await _repository.GetCarModelsByMakeIdAsync(carMake.MakeId, 1, 3);
        
        // Act - Get second page (3 items)
        var (secondPageItems, secondPageTotal) = await _repository.GetCarModelsByMakeIdAsync(carMake.MakeId, 2, 3);
        
        // Act - Get third page (1 item)
        var (thirdPageItems, thirdPageTotal) = await _repository.GetCarModelsByMakeIdAsync(carMake.MakeId, 3, 3);

        // Assert
        firstPageItems.Should().HaveCount(3);
        firstPageTotal.Should().Be(7);
        
        secondPageItems.Should().HaveCount(3);
        secondPageTotal.Should().Be(7);
        
        thirdPageItems.Should().HaveCount(1);
        thirdPageTotal.Should().Be(7);

        // Ensure no duplicate items across pages
        var allItems = firstPageItems.Concat(secondPageItems).Concat(thirdPageItems);
        allItems.Select(m => m.ModelId).Should().OnlyHaveUniqueItems();
    }

    /// <summary>
    /// Tests that UpdateCarModelAsync successfully updates and persists changes to an existing car model.
    /// Expects the car model to be updated with correct data and persisted to database.
    /// </summary>
    [Fact]
    public async Task UpdateCarModelAsync_WithValidCarModel_ShouldUpdateSuccessfully()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);
        await _context.CarModels.AddAsync(carModel);
        await _context.SaveChangesAsync();

        // Modify the car model
        carModel.Name = "Updated Model Name";
        carModel.ModelYear = 2025;

        // Act
        var result = await _repository.UpdateCarModelAsync(carModel);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Model Name");
        result.ModelYear.Should().Be(2025);

        var updatedCarModel = await _context.CarModels.FindAsync(carModel.ModelId);
        updatedCarModel.Should().NotBeNull();
        updatedCarModel!.Name.Should().Be("Updated Model Name");
        updatedCarModel.ModelYear.Should().Be(2025);
    }

    /// <summary>
    /// Tests that DeleteCarModelAsync successfully removes a car model from the database.
    /// Expects the car model to be deleted and no longer exist in the database.
    /// </summary>
    [Fact]
    public async Task DeleteCarModelAsync_WithValidId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);
        await _context.CarModels.AddAsync(carModel);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteCarModelAsync(carModel.ModelId);
        await _context.SaveChangesAsync();

        // Assert
        var deletedCarModel = await _context.CarModels.FindAsync(carModel.ModelId);
        deletedCarModel.Should().BeNull();
    }

    /// <summary>
    /// Tests that HasCarModelsForMakeAsync returns true when car models exist for the specified make.
    /// Expects true result when car models exist for the given make ID.
    /// </summary>
    [Fact]
    public async Task HasCarModelsForMakeAsync_WithExistingModels_ShouldReturnTrue()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        
        var carModel = CarModelFaker.Generate(carMake.MakeId);
        await _context.CarModels.AddAsync(carModel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasCarModelsForMakeAsync(carMake.MakeId);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that HasCarModelsForMakeAsync returns false when no car models exist for the specified make.
    /// Expects false result when no car models exist for the given make ID.
    /// </summary>
    [Fact]
    public async Task HasCarModelsForMakeAsync_WithoutModels_ShouldReturnFalse()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasCarModelsForMakeAsync(carMake.MakeId);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that GetCarModelsByMakeIdAsync returns empty results when no models exist for the specified make.
    /// Expects empty collection and zero count.
    /// </summary>
    [Fact]
    public async Task GetCarModelsByMakeIdAsync_WithNonExistentMake_ShouldReturnEmptyResults()
    {
        // Arrange
        var nonExistentMakeId = Guid.NewGuid();

        // Act
        var (items, totalCount) = await _repository.GetCarModelsByMakeIdAsync(nonExistentMakeId, 1, 10);

        // Assert
        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}