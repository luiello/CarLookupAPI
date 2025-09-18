using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CarLookup.Tests.Integration.Pagination;

/// <summary>
/// Integration tests for the modern pagination system.
/// Tests pagination functionality end-to-end including parameter handling, response format, and link generation.
/// </summary>
public class PaginationIntegrationTests : IClassFixture<WebApplicationFactory<CarLookup.Host.Program>>
{
    private readonly WebApplicationFactory<CarLookup.Host.Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _output;

    public PaginationIntegrationTests(WebApplicationFactory<CarLookup.Host.Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Tests that car makes endpoint returns correct pagination format with page and limit parameters.
    /// Expects proper pagination metadata including current page, limit, total items, and navigation links.
    /// </summary>
    [Fact]
    public async Task GetCarMakes_WithPageAndLimit_ShouldReturnCorrectPaginationFormat()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act - Use modern pagination parameters
        var response = await client.GetAsync("/api/v1/carmakes?page=1&limit=5");

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response status: {response.StatusCode}");
            _output.WriteLine($"Response content: {error}");
        }
        
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DummyPagedCarMakeResponse>(content, _jsonOptions);

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Pagination.Should().NotBeNull();
        result.Pagination!.CurrentPage.Should().Be(1);
        result.Pagination.Limit.Should().Be(5);
        result.Pagination.TotalItems.Should().BeGreaterOrEqualTo(0);
        
        if (result.Pagination.TotalItems > 5)
        {
            result.Pagination.NextPage.Should().Contain("page=2");
            result.Pagination.NextPage.Should().Contain("limit=5");
        }
        result.Pagination.PrevPage.Should().BeNull();
    }

    /// <summary>
    /// Tests that default page size is applied when limit parameter is not specified.
    /// Expects the configured default page size (20) to be used when no limit is provided.
    /// </summary>
    [Fact]
    public async Task GetCarMakes_WithDefaultPageSize_ShouldUseConfiguredDefault()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act - Don't specify limit to test default behavior
        var response = await client.GetAsync("/api/v1/carmakes?page=1");

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response status: {response.StatusCode}");
            _output.WriteLine($"Response content: {error}");
        }
        
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DummyPagedCarMakeResponse>(content, _jsonOptions);

        result.Should().NotBeNull();
        result!.Pagination.Should().NotBeNull();
        result.Pagination!.Limit.Should().Be(20); // Default from PaginationOptions
    }

    /// <summary>
    /// Tests that excessive page size returns validation error instead of being clamped.
    /// Expects BadRequest status when page size exceeds maximum allowed value.
    /// </summary>
    [Fact]
    public async Task GetCarMakes_WithExcessivePageSize_ShouldReturnValidationError()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/carmakes?page=1&limit=500");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
        
        errorResult.Should().NotBeNull();
        errorResult!.Success.Should().BeFalse();
        errorResult.Message.Should().Contain("Validation failed");
    }

    /// <summary>
    /// Tests that name filter parameter is preserved in pagination navigation links.
    /// Expects filter parameters to be included in next/previous page URLs for consistent navigation.
    /// </summary>
    [Fact]
    public async Task GetCarMakes_WithNameFilter_ShouldIncludeFilterInPaginationLinks()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act - Test with name filter
        var response = await client.GetAsync("/api/v1/carmakes?page=1&limit=2&nameContains=Toyota");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DummyPagedCarMakeResponse>(content, _jsonOptions);

        result.Should().NotBeNull();
        result!.Pagination.Should().NotBeNull();
        
        if (result.Pagination!.NextPage != null)
        {
            result.Pagination.NextPage.Should().Contain("nameContains=Toyota");
            result.Pagination.NextPage.Should().Contain("page=2");
            result.Pagination.NextPage.Should().Contain("limit=2");
        }
    }

    private async Task<string> GetReaderTokenStubAsync(HttpClient client)
    {
        var loginRequest = new LoginRequest
        {
            Username = "reader",
            Password = "reader123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/token", loginRequest);
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>(_jsonOptions);
        return apiResponse!.Data!.AccessToken;
    }
}

/// <summary>
/// Dummy response type for deserializing car make pagination responses in tests
/// </summary>
public class DummyPagedCarMakeResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object[] Data { get; set; }
    public PaginationInfo Pagination { get; set; }
}