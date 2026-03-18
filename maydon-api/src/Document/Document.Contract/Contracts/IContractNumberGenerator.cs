namespace Document.Contract.Contracts;

/// <summary>
/// Generates sequential contract numbers per tenant per year.
/// Format: {YYYY}-{sequence:D6} (e.g. 2026-000001).
/// </summary>
public interface IContractNumberGenerator
{
    /// <summary>
    /// Generates the next contract number for the given tenant.
    /// Thread-safe via database-level MAX() + 1 approach.
    /// </summary>
    Task<string> GenerateNextAsync(Guid tenantId, CancellationToken ct = default);
}
