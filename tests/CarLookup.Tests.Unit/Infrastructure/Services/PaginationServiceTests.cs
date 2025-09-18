using CarLookup.Contracts.Requests;
using CarLookup.Infrastructure.Options;
using CarLookup.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;

namespace CarLookup.Tests.Unit.Infrastructure.Services;

/// <summary>
/// Unit tests for PaginationService
/// </summary>
public class PaginationServiceTests
{
    private readonly PaginationOptions _options;
    private readonly PaginationService _paginationService;

    public PaginationServiceTests()
    {
        _options = new PaginationOptions
        {
            DefaultPageSize = 20,
            MaxPageSize = 100,
            MaxNameFilterLength = 100
        };

        var optionsWrapper = Options.Create(_options);
        _paginationService = new PaginationService(optionsWrapper);
    }

    [Fact]
    public void Clamp_PaginationQuery_ShouldClampPageNumberToMinimum()
    {
        // Arrange
        var query = new PaginationQuery { Page = 0, Limit = 10 };

        // Act
        var result = _paginationService.Clamp(query);

        // Assert
        result.Page.Should().Be(1);
        result.Limit.Should().Be(10);
    }

    [Fact]
    public void Clamp_PaginationQuery_ShouldUseDefaultPageSizeWhenZero()
    {
        // Arrange
        var query = new PaginationQuery { Page = 1, Limit = 0 };

        // Act
        var result = _paginationService.Clamp(query);

        // Assert
        result.Page.Should().Be(1);
        result.Limit.Should().Be(_options.DefaultPageSize);
    }

    [Fact]
    public void Clamp_PaginationQuery_ShouldClampPageSizeToMaximum()
    {
        // Arrange
        var query = new PaginationQuery { Page = 1, Limit = 150 };

        // Act
        var result = _paginationService.Clamp(query);

        // Assert
        result.Page.Should().Be(1);
        result.Limit.Should().Be(_options.MaxPageSize);
    }

    [Fact]
    public void Clamp_PaginationQuery_ShouldPreserveNameContains()
    {
        // Arrange
        var query = new PaginationQuery 
        { 
            Page = 2, 
            Limit = 30, 
            NameContains = "Toyota" 
        };

        // Act
        var result = _paginationService.Clamp(query);

        // Assert
        result.Page.Should().Be(2);
        result.Limit.Should().Be(30);
        result.NameContains.Should().Be("Toyota");
    }

    [Fact]
    public void Clamp_CarModelPaginationQuery_ShouldPreserveYearFilter()
    {
        // Arrange
        var query = new CarModelPaginationQuery 
        { 
            Page = 1, 
            Limit = 25, 
            NameContains = "Camry",
            Year = 2023
        };

        // Act
        var result = _paginationService.Clamp(query);

        // Assert
        result.Page.Should().Be(1);
        result.Limit.Should().Be(25);
        result.NameContains.Should().Be("Camry");
        result.Year.Should().Be(2023);
    }

    [Fact]
    public void CreatePaginationInfo_WithEmptyResults_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var page = 1;
        var limit = 10;
        var totalItems = 0;
        var basePath = "/api/v1/carmakes";

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath);

        // Assert
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(0);
        result.TotalItems.Should().Be(0);
        result.Limit.Should().Be(10);
        result.NextPage.Should().BeNull();
        result.PrevPage.Should().BeNull();
    }

    [Fact]
    public void CreatePaginationInfo_WithFirstPageResults_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var page = 1;
        var limit = 10;
        var totalItems = 25;
        var basePath = "/api/v1/carmakes";

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath);

        // Assert
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(3);
        result.TotalItems.Should().Be(25);
        result.Limit.Should().Be(10);
        result.NextPage.Should().Be("/api/v1/carmakes?page=2&limit=10");
        result.PrevPage.Should().BeNull();
    }

    [Fact]
    public void CreatePaginationInfo_WithMiddlePageResults_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var page = 2;
        var limit = 10;
        var totalItems = 35;
        var basePath = "/api/v1/carmakes";

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath);

        // Assert
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(4);
        result.TotalItems.Should().Be(35);
        result.Limit.Should().Be(10);
        result.NextPage.Should().Be("/api/v1/carmakes?page=3&limit=10");
        result.PrevPage.Should().Be("/api/v1/carmakes?page=1&limit=10");
    }

    [Fact]
    public void CreatePaginationInfo_WithLastPageResults_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var page = 3;
        var limit = 10;
        var totalItems = 25;
        var basePath = "/api/v1/carmakes";

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath);

        // Assert
        result.CurrentPage.Should().Be(3);
        result.TotalPages.Should().Be(3);
        result.TotalItems.Should().Be(25);
        result.Limit.Should().Be(10);
        result.NextPage.Should().BeNull();
        result.PrevPage.Should().Be("/api/v1/carmakes?page=2&limit=10");
    }

    [Fact]
    public void CreatePaginationInfo_WithExtraQueryParameters_ShouldIncludeInUrls()
    {
        // Arrange
        var page = 1;
        var limit = 10;
        var totalItems = 25;
        var basePath = "/api/v1/carmakes";
        var extraQuery = new Dictionary<string, object>
        {
            ["nameContains"] = "Toyota",
            ["year"] = 2023
        };

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath, extraQuery);

        // Assert
        result.NextPage.Should().Contain("nameContains=Toyota");
        result.NextPage.Should().Contain("year=2023");
        result.NextPage.Should().Contain("page=2");
        result.NextPage.Should().Contain("limit=10");
    }

    [Fact]
    public void CreatePaginationInfo_WithNullOrEmptyExtraQuery_ShouldIgnoreThem()
    {
        // Arrange
        var page = 1;
        var limit = 10;
        var totalItems = 25;
        var basePath = "/api/v1/carmakes";
        var extraQuery = new Dictionary<string, object>
        {
            ["nameContains"] = null,
            ["emptyString"] = "",
            ["whitespace"] = "   ",
            ["validValue"] = "Toyota"
        };

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath, extraQuery);

        // Assert
        result.NextPage.Should().NotContain("nameContains");
        result.NextPage.Should().NotContain("emptyString");
        result.NextPage.Should().NotContain("whitespace");
        result.NextPage.Should().Contain("validValue=Toyota");
    }

    [Fact]
    public void CreatePaginationInfo_WithPageNumberExceedingTotalPages_ShouldClampCurrentPage()
    {
        // Arrange
        var page = 10;
        var limit = 10;
        var totalItems = 25;
        var basePath = "/api/v1/carmakes";

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath);

        // Assert
        result.CurrentPage.Should().Be(3); // Clamped to last available page
        result.TotalPages.Should().Be(3);
        result.NextPage.Should().BeNull(); // No next page since we're on the last page
        result.PrevPage.Should().Be("/api/v1/carmakes?page=2&limit=10");
    }

    [Fact]
    public void CreatePaginationInfo_WithSpecialCharactersInQueryParams_ShouldUrlEncode()
    {
        // Arrange
        var page = 1;
        var limit = 10;
        var totalItems = 25;
        var basePath = "/api/v1/carmakes";
        var extraQuery = new Dictionary<string, object>
        {
            ["nameContains"] = "Aston Martin & Co.",
        };

        // Act
        var result = _paginationService.CreatePaginationInfo(page, limit, totalItems, basePath, extraQuery);

        // Assert
        result.NextPage.Should().Contain("nameContains=Aston%20Martin%20%26%20Co.");
    }
}