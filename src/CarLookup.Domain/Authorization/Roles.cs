namespace CarLookup.Domain.Authorization
{
    /// <summary>
    /// Application base roles (as issued in JWT role claims)
    /// </summary>
    public static class Roles
    {
        public const string Admin = "admin";
        public const string Editor = "editor";
        public const string Reader = "reader";
    }
}
