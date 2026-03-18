using Document.Application.Abstractions.Data;
using Document.Contract.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Document.Infrastructure.Services;

/// <summary>
/// Generates sequential contract numbers per tenant per year.
/// Uses database-level MAX() to determine the next sequence.
/// Format: {YYYY}-{sequence:D6} (e.g. 2026-000001).
/// </summary>
public sealed class ContractNumberGenerator(IDocumentDbContext dbContext) : IContractNumberGenerator
{
    public async Task<string> GenerateNextAsync(Guid tenantId, CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"{year}-";

        // Find the max contract number for this tenant in the current year
        var lastNumber = await dbContext.Contracts
            .AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.ContractNumber != null && c.ContractNumber.StartsWith(prefix))
            .Select(c => c.ContractNumber!)
            .MaxAsync(ct)
            .ConfigureAwait(false);

        var nextSequence = 1;

        if (lastNumber is not null)
        {
            // Extract the numeric part after "YYYY-"
            var numericPart = lastNumber.AsSpan(prefix.Length);
            if (int.TryParse(numericPart, out var current))
            {
                nextSequence = current + 1;
            }
        }

        return $"{prefix}{nextSequence:D6}";
    }
}
