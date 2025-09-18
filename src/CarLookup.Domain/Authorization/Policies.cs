namespace CarLookup.Domain.Authorization;

/// <summary>
/// Named authorization policies
/// </summary>
public static class Policies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string EditorOrAbove = nameof(EditorOrAbove);
    public const string ReaderOrAbove = nameof(ReaderOrAbove);
}
