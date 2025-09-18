namespace CarLookup.Access.Extensions;

/// <summary>
/// Extension methods for Entity Framework operations in the Access layer
/// </summary>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Escapes special LIKE pattern characters to prevent SQL injection and unintended pattern matching.
    /// </summary>
    /// <param name="input">The input string to escape</param>
    /// <returns>A string with LIKE pattern characters properly escaped</returns>
    public static string EscapeLikePattern(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input
            .Replace("[", "[[")  // Escape bracket first to avoid double-escaping
            .Replace("%", "[%]")  // Escape percent wildcard
            .Replace("_", "[_]"); // Escape underscore wildcard
    }

    /// <summary>
    /// Creates a LIKE pattern with escaped user input for safe SQL queries.
    /// </summary>
    /// <param name="input">The search term to wrap</param>
    /// <returns>A safe LIKE pattern in the format <code>%{escaped_input}%</code></returns>
    public static string ToContainsPattern(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "%";

        var escaped = input.Trim().EscapeLikePattern();
        return $"%{escaped}%";
    }
}