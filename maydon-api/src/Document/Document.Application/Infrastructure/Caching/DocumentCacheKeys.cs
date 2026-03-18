namespace Document.Application.Infrastructure.Caching;

/// <summary>
/// Cache key constants and helpers for Document module
/// </summary>
public static class DocumentCacheKeys
{
    /// <summary>
    /// Prefix for DocumentTemplate-related cache keys
    /// </summary>
    private const string TemplatePrefix = "tmpl";

    private static string Build(string prefix, Guid id) => $"{prefix}:{id}";

    // Template-specific helpers
    public static string Template(Guid id) => Build(TemplatePrefix, id);
}

