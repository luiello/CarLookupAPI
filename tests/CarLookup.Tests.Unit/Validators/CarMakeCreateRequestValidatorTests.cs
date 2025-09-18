using CarLookup.Manager.Validators;
using CarLookup.TestData.Fakers;
using FluentAssertions;
using Xunit;

namespace CarLookup.Tests.Unit.Validators;

/// <summary>
/// Unit tests for CarMakeCreateRequestValidator following AAA pattern
/// </summary>
public class CarMakeCreateRequestValidatorTests
{
    private readonly CarMakeCreateRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var request = RequestFakers.CarMakeRequest().Generate();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyOrNullName_ShouldReturnFalse(string name)
    {
        // Arrange
        var request = RequestFakers.CarMakeRequest().Generate();
        request.Name = name;

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Name));
    }

    [Fact]
    public void Validate_NameTooShort_ShouldReturnFalse()
    {
        // Arrange
        var request = RequestFakers.CarMakeRequest().Generate();
        request.Name = "A";

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Name));
    }

    [Fact]
    public void Validate_NameTooLong_ShouldReturnFalse()
    {
        // Arrange
        var request = RequestFakers.CarMakeRequest().Generate();
        request.Name = new string('A', 101);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyOrNullCountryOfOrigin_ShouldReturnFalse(string country)
    {
        // Arrange
        var request = RequestFakers.CarMakeRequest().Generate();
        request.CountryOfOrigin = country;

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.CountryOfOrigin));
    }
}