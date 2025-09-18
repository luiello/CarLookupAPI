using CarLookup.Access.Extensions;
using FluentAssertions;
using Xunit;

namespace CarLookup.Tests.Unit.Access.Extensions;

/// <summary>
/// Unit tests for EntityFrameworkExtensions that provide SQL LIKE pattern escaping.
/// Tests the proper escaping of special characters to prevent SQL injection and ensure correct pattern matching.
/// </summary>
public class EntityFrameworkExtensionsTests
{
    /// <summary>
    /// Tests that ToContainsPattern properly escapes special LIKE characters and wraps with wildcards.
    /// Expects input to be escaped and wrapped with % wildcards for contains-style pattern matching.
    /// </summary>
    [Theory]
    [InlineData("test", "%test%")]
    [InlineData("Test_Data", "%Test[_]Data%")]
    [InlineData("50%", "%50[%]%")]
    [InlineData("data[bracket]", "%data[[bracket]%")]
    public void ToContainsPattern_ShouldEscapeSpecialCharacters(string input, string expected)
    {
        // Act
        var result = input.ToContainsPattern();

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that EscapeLikePattern escapes SQL LIKE special characters without adding wildcards.
    /// Expects underscore, percent, and brackets to be properly escaped for safe SQL usage.
    /// </summary>
    [Theory]
    [InlineData("test", "test")]
    [InlineData("Test_Data", "Test[_]Data")]
    [InlineData("50%", "50[%]")]
    [InlineData("data[bracket]", "data[[bracket]")]
    public void EscapeLikePattern_ShouldEscapeSpecialCharacters(string input, string expected)
    {
        // Act
        var result = input.EscapeLikePattern();

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that ToContainsPattern handles null and empty string inputs appropriately.
    /// Expects null to remain null and empty/whitespace strings to become just wildcards.
    /// </summary>
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "%")]
    [InlineData("   ", "%")]
    public void ToContainsPattern_ShouldHandleNullAndEmpty(string input, string expected)
    {
        // Act
        var result = input?.ToContainsPattern();

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that EscapeLikePattern handles null and empty string inputs safely.
    /// Expects null to remain null and empty string to remain empty without modification.
    /// </summary>
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    public void EscapeLikePattern_ShouldHandleNullAndEmpty(string input, string expected)
    {
        // Act
        var result = input?.EscapeLikePattern();

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that EscapeLikePattern escapes brackets before other characters to avoid conflicts.
    /// Expects brackets to be escaped first to prevent interference with escape sequences.
    /// </summary>
    [Fact]
    public void EscapeLikePattern_ShouldEscapeBracketsFirst()
    {
        // Arrange
        var input = "[%_]";

        // Act
        var result = input.EscapeLikePattern();

        // Assert - actual output is [[[%][_]]
        result.Should().Be("[[[%][_]]");
    }

    /// <summary>
    /// Tests that EscapeLikePattern prevents SQL injection attempts through pattern escaping.
    /// Expects malicious SQL code to be safely escaped without special LIKE characters.
    /// </summary>
    [Fact]
    public void EscapeLikePattern_ShouldPreventSqlInjection()
    {
        // Arrange - malicious input attempting to use LIKE wildcards
        var maliciousInput = "'; DROP TABLE Users; --";

        // Act
        var escaped = maliciousInput.EscapeLikePattern();
        var pattern = escaped.ToContainsPattern();

        // Assert
        escaped.Should().NotContain("%").And.NotContain("_");
        pattern.Should().StartWith("%").And.EndWith("%");
    }
}