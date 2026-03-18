using Core.Domain.Entities;
using Document.Domain.Shared;

namespace Document.Domain.Contracts;

public sealed class ContractProviderState : ValueObject
{
    public string ProviderName { get; }
    public string? ExternalId { get; }
    public ExternalSyncStatus SyncStatus { get; }
    public DateTime LastUpdated { get; }
    public string? ErrorMessage { get; }

    private ContractProviderState() { } // EF Core

    public ContractProviderState(
        string providerName,
        string? externalId,
        ExternalSyncStatus status,
        DateTime lastUpdated,
        string? errorMessage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);

        ProviderName = providerName;
        ExternalId = externalId;
        SyncStatus = status;
        LastUpdated = lastUpdated;
        ErrorMessage = errorMessage;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProviderName;
        yield return ExternalId ?? string.Empty;
        yield return SyncStatus;
        yield return LastUpdated;
    }
}
