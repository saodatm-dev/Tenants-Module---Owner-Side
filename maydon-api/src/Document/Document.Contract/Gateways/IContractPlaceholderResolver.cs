namespace Document.Contract.Gateways;

/// <summary>
/// Resolves all auto-resolvable placeholders for contract generation.
/// Combines data from Lease, RealEstate (Building module), Company/User (Identity module).
/// </summary>
public interface IContractPlaceholderResolver
{
    /// <summary>
    /// Resolves placeholders from all data sources for the given lease.
    /// Returns a flat dictionary keyed by placeholder name (e.g. "owner_company_name", "lease_monthly_rent").
    /// </summary>
    Task<Dictionary<string, object?>> ResolveAllAsync(Guid leaseId, Guid tenantId, CancellationToken ct = default);
}
