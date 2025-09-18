using CarLookup.Access;
using CarLookup.TestData.Fakers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CarLookup.Tests.Integration.Services;

/// <summary>
/// Integration tests for database transaction handling and ACID compliance.
/// Tests the UnitOfWork pattern ensuring proper rollback and commit behavior.
/// </summary>
public class TransactionTests : IDisposable
{
    private readonly CarLookupDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public TransactionTests()
    {
        var options = new DbContextOptionsBuilder<CarLookupDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var contextLogger = Mock.Of<ILogger<CarLookupDbContext>>();
        _context = new CarLookupDbContext(options, contextLogger);

        var loggerFactory = Mock.Of<ILoggerFactory>();
        Mock.Get(loggerFactory)
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());

        _unitOfWork = new UnitOfWork(_context, loggerFactory);
    }

    /// <summary>
    /// Tests that database transaction properly rolls back when an operation fails.
    /// Expects no changes to be persisted when an exception occurs within a transaction.
    /// </summary>
    [Fact]
    public async Task Transaction_WhenOperationFails_ShouldRollback()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        await _context.CarMakes.AddAsync(carMake);
        await _context.SaveChangesAsync();

        var initialCount = await _context.CarMakes.CountAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Add a car make
                var newCarMake = CarMakeFaker.Generate();
                await _unitOfWork.MakeRepository.CreateCarMakeAsync(newCarMake);

                // This should cause the transaction to rollback
                throw new InvalidOperationException("Simulated error");
            });
        });

        // Verify rollback - count should be unchanged
        var finalCount = await _context.CarMakes.CountAsync();
        finalCount.Should().Be(initialCount);
    }

    /// <summary>
    /// Tests that database transaction properly commits when operation completes successfully.
    /// Expects all changes to be persisted when no exceptions occur within a transaction.
    /// </summary>
    [Fact]
    public async Task Transaction_WhenOperationSucceeds_ShouldCommit()
    {
        // Arrange
        var initialCount = await _context.CarMakes.CountAsync();

        // Act
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var newCarMake = CarMakeFaker.Generate();
            await _unitOfWork.MakeRepository.CreateCarMakeAsync(newCarMake);
        });

        // Assert
        var finalCount = await _context.CarMakes.CountAsync();
        finalCount.Should().Be(initialCount + 1);
    }

    /// <summary>
    /// Tests that database operations can be executed within the UnitOfWork pattern.
    /// Expects operations to complete successfully even when transactions aren't supported (in-memory).
    /// </summary>
    [Fact]
    public async Task UnitOfWork_WithInMemoryDatabase_ShouldExecuteOperationsSuccessfully()
    {
        // Arrange
        var initialCount = await _context.CarMakes.CountAsync();
        var newCarMake = CarMakeFaker.Generate();

        // Act - Execute operation directly since in-memory DB doesn't support transactions
        await _unitOfWork.MakeRepository.CreateCarMakeAsync(newCarMake);
        await _context.SaveChangesAsync(); // Ensure changes are committed

        // Assert
        var finalCount = await _context.CarMakes.CountAsync();
        finalCount.Should().Be(initialCount + 1);
    }

    /// <summary>
    /// Tests that multiple operations can be performed through the UnitOfWork.
    /// Expects all operations to be properly coordinated through the unit of work pattern.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_WithMultipleOperations_ShouldCoordinateSuccessfully()
    {
        // Arrange
        var carMake1 = CarMakeFaker.Generate();
        var carMake2 = CarMakeFaker.Generate();

        // Act
        await _unitOfWork.MakeRepository.CreateCarMakeAsync(carMake1);
        await _unitOfWork.MakeRepository.CreateCarMakeAsync(carMake2);
        await _context.SaveChangesAsync(); // Ensure changes are committed

        // Assert
        var retrievedMake1 = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(carMake1.MakeId);
        var retrievedMake2 = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(carMake2.MakeId);

        retrievedMake1.Should().NotBeNull();
        retrievedMake2.Should().NotBeNull();
        retrievedMake1!.Name.Should().Be(carMake1.Name);
        retrievedMake2!.Name.Should().Be(carMake2.Name);
    }

    /// <summary>
    /// Tests that repository operations through UnitOfWork maintain data consistency.
    /// Expects that create and read operations work correctly together.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_WithRepositoryOperations_ShouldMaintainConsistency()
    {
        // Arrange
        var carMake = CarMakeFaker.Generate();
        
        // Act
        var createdMake = await _unitOfWork.MakeRepository.CreateCarMakeAsync(carMake);
        await _context.SaveChangesAsync(); // Ensure changes are committed
        
        var retrievedMake = await _unitOfWork.MakeRepository.GetCarMakeByIdAsync(createdMake.MakeId);
        var exists = await _unitOfWork.MakeRepository.ExistsByNameAsync(carMake.Name);

        // Assert
        retrievedMake.Should().NotBeNull();
        retrievedMake!.MakeId.Should().Be(carMake.MakeId);
        retrievedMake.Name.Should().Be(carMake.Name);
        exists.Should().BeTrue();
    }

    public void Dispose()
    {
        _unitOfWork?.Dispose();
        _context?.Dispose();
    }
}