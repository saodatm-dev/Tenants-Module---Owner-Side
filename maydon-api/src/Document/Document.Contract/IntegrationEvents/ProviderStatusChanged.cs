using Core.Application.Abstractions.Messaging;
using Document.Contract.Enums;

namespace Document.Contract.IntegrationEvents;

/// <summary>
/// Integration event raised when a provider's sync status changes.
/// V2: Added InitiatedBy to preserve user context for versioning
/// </summary>
public record ProviderStatusChanged(
    Guid DocumentId,
    string ProviderName,
    string? ExternalId,
    ExternalSyncStatus Status,
    string? ErrorMessage,
    Guid? InitiatedBy = null
) : IntegrationEventBase
{
    public override int SchemaVersion => 2;
}
