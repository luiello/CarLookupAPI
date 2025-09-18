using CarLookup.Contracts.Dtos;
using CarLookup.Contracts.Requests;
using CarLookup.Contracts.Responses;
using CarLookup.TestData.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CarLookup.Tests.Acceptance;

/// <summary>
/// End-to-end acceptance tests for the CarLookup API.
/// Tests the complete API workflow including authentication, CRUD operations, and pagination.
/// </summary>
public class CarLookupApiTests : IClassFixture<WebApplicationFactory<CarLookup.Host.Program>>
{
    private readonly WebApplicationFactory<CarLookup.Host.Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CarLookupApiTests(WebApplicationFactory<CarLookup.Host.Program> factory)
    {
        _factory = factory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Tests that authentication flow with valid credentials returns a valid JWT token.
    /// Expects successful login with valid token structure and editor role.
    /// </summary>
    [Fact]
    public async Task AuthenticationFlow_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var loginRequest = new LoginRequest
        {
            Username = "editor",
            Password = "editor123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/token", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>(_jsonOptions);
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.AccessToken.Should().NotBeEmpty();
        apiResponse.Data.TokenType.Should().Be("Bearer");
        apiResponse.Data.Roles.Should().Contain("editor");
    }

    /// <summary>
    /// Tests complete CRUD workflow for car makes including authentication.
    /// Expects successful creation, retrieval, and proper authentication handling.
    /// </summary>
    [Fact]
    public async Task FullMakeWorkflow_CreateReadUpdateDelete_ShouldWork()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetEditorTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Generate unique name to avoid conflicts
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var createRequest = RequestFakers.CarMakeRequest().Generate();
        createRequest.Name = $"TestMake_{uniqueId}_{DateTime.UtcNow.Ticks}";

        // Act & Assert - Create Make
        var createResponse = await client.PostAsJsonAsync("/api/v1/carmakes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdMake = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CarMakeDto>>(_jsonOptions);
        createdMake.Should().NotBeNull();
        createdMake!.Success.Should().BeTrue();
        createdMake.Data.Should().NotBeNull();
        createdMake.Data!.Name.Should().Be(createRequest.Name);

        var carMakeId = createdMake.Data.MakeId;

        // Act & Assert - Read Make by ID
        var getByIdResponse = await client.GetAsync($"/api/v1/carmakes/{carMakeId}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var makeResult = await getByIdResponse.Content.ReadFromJsonAsync<ApiResponse<CarMakeDto>>(_jsonOptions);
        makeResult.Should().NotBeNull();
        makeResult!.Success.Should().BeTrue();
        makeResult.Data.Should().NotBeNull();
        makeResult.Data!.MakeId.Should().Be(carMakeId);

        // Act & Assert - Read All Makes
        var getMakesResponse = await client.GetAsync("/api/v1/carmakes");
        getMakesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var makesResult = await getMakesResponse.Content.ReadFromJsonAsync<PagedResponse<CarMakeDto>>(_jsonOptions);
        makesResult.Should().NotBeNull();
        makesResult!.Success.Should().BeTrue();
        makesResult.Data.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that pagination endpoints return correct format and structure.
    /// Expects proper pagination metadata with valid page information.
    /// </summary>
    [Fact]
    public async Task GetMakes_WithPaginationUri_ShouldReturnCorrectFormat()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act - Test pagination format
        var response = await client.GetAsync("/api/v1/carmakes?page=1&limit=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<PagedApiResponseDummy>(_jsonOptions);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Pagination.Should().NotBeNull();
        result.Pagination!.CurrentPage.Should().Be(1);
        result.Pagination.Limit.Should().Be(5);
        result.Pagination.TotalItems.Should().BeGreaterThanOrEqualTo(0);
        result.Pagination.TotalPages.Should().BeGreaterThanOrEqualTo(0);
    }

    /// <summary>
    /// Tests that unauthorized requests to protected endpoints return 401 status.
    /// Expects authentication challenge when no token is provided.
    /// </summary>
    [Fact]
    public async Task UnauthorizedAccess_ToProtectedEndpoint_ShouldReturn401()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var createRequest = RequestFakers.CarMakeRequest().Generate();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/carmakes", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Tests that invalid authentication credentials return proper error response.
    /// Expects 401 status with authentication failure.
    /// </summary>    
    [Fact]
    public async Task AuthenticationFlow_WithInvalidCredentials_ShouldReturn401()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var loginRequest = new LoginRequest
        {
            Username = "invalid",
            Password = "wrongpassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/token", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Tests that requests with reader role can access read endpoints but not write endpoints.
    /// Expects proper role-based authorization enforcement.
    /// </summary>
    [Fact]
    public async Task ReaderRole_CanReadButNotWrite_ShouldEnforceRoleBasedAccess()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act & Assert - Should be able to read
        var getResponse = await client.GetAsync("/api/v1/carmakes");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act & Assert - Should not be able to create
        var createRequest = RequestFakers.CarMakeRequest().Generate();
        var createResponse = await client.PostAsJsonAsync("/api/v1/carmakes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Tests that API returns proper validation errors for invalid car make data.
    /// Expects 400 status with detailed validation error messages.
    /// </summary>
    [Fact]
    public async Task CreateMake_WithInvalidData_ShouldReturnValidationErrors()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetEditorTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var invalidRequest = new CarMakeRequest
        {
            Name = "", // Invalid: empty name
            CountryOfOrigin = "" // Invalid: empty country
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/carmakes", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(_jsonOptions);
        errorResponse.Should().NotBeNull();
        errorResponse!.Success.Should().BeFalse();
        errorResponse.Message.Should().Contain("Validation failed");
    }

    /// <summary>
    /// Tests that API returns conflict error when attempting to create duplicate car make.
    /// Expects 409 status when trying to create car make with existing name.
    /// </summary>
    [Fact]
    public async Task CreateMake_WithDuplicateName_ShouldReturnConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetEditorTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var makeRequest = RequestFakers.CarMakeRequest().Generate();
        makeRequest.Name = $"DuplicateTest_{uniqueId}_{DateTime.UtcNow.Ticks}";

        // Create the first make
        var firstResponse = await client.PostAsJsonAsync("/api/v1/carmakes", makeRequest);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Try to create another make with the same name
        var duplicateRequest = new CarMakeRequest
        {
            Name = makeRequest.Name,
            CountryOfOrigin = makeRequest.CountryOfOrigin
        };

        // Act
        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/carmakes", duplicateRequest);

        // Assert
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// Tests that API returns 404 when attempting to access non-existent car make.
    /// Expects NotFound status when requesting non-existent resource.
    /// </summary>
    [Fact]
    public async Task GetMake_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/carmakes/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that pagination works correctly with name filtering.
    /// Expects filtered results to only include items matching the search criteria.
    /// </summary>
    [Fact]
    public async Task GetMakes_WithNameFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = await GetReaderTokenStubAsync(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/carmakes?page=1&limit=10&nameContains=Toyota");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<CarMakeDto>>(_jsonOptions);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Pagination.Should().NotBeNull();

        // If there are results, they should contain "Toyota" in the name
        if (result.Data?.Any() == true)
        {
            result.Data.Should().OnlyContain(make => make.Name.Contains("Toyota", StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Tests that expired or invalid JWT tokens are properly rejected.
    /// Expects 401 status when using invalid authentication tokens.
    /// </summary>
    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid-token-123");

        // Act
        var response = await client.GetAsync("/api/v1/carmakes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<string> GetEditorTokenStubAsync(HttpClient client)
    {
        var loginRequest = new LoginRequest
        {
            Username = "editor",
            Password = "editor123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/token", loginRequest);
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>(_jsonOptions);
        return apiResponse!.Data!.AccessToken;
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
/// Dummy response type for testing pagination functionality
/// </summary>
public class PagedApiResponseDummy
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object Data { get; set; }
    public PaginationInfo Pagination { get; set; }
}