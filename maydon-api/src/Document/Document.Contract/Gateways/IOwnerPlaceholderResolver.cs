namespace Document.Contract.Gateways;

/// <summary>
/// Resolves owner/company placeholders from backend data sources (Identity module).
/// Used by the preview and PDF generation handlers.
/// </summary>
public interface IOwnerPlaceholderResolver
{
    /// <summary>
    /// Resolves all auto-resolvable owner placeholders for the given tenant.
    /// Returns a dictionary of placeholder keys to their resolved values.
    /// </summary>
    Task<Dictionary<string, object?>> ResolveOwnerValuesAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
