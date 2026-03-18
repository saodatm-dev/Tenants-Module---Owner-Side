using Core.Application.Abstractions.Messaging;
using Document.Application.Abstractions.Data;
using Document.Contract.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DomainSyncStatus = Document.Domain.Shared.ExternalSyncStatus;

namespace Document.Application.Features.Contracts.IntegrationEvents;

/// <summary>
/// Handles <see cref="ProviderStatusChanged"/> integration events.
/// Updates the contract's integration state (provider status + external ID)
/// on the Document side when the Didox worker reports a status change.
/// </summary>
public sealed class ProviderStatusChangedHandler(
    IDocumentDbContext db,
    ILogger<ProviderStatusChangedHandler> logger)
    : IntegrationEventHandlerBase<ProviderStatusChanged>(logger)
{
    protected override async Task HandleCoreAsync(
        ProviderStatusChanged @event,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "ProviderStatusChanged: Document {DocumentId}, Provider {Provider}, Status {Status}, ExternalId {ExternalId}",
            @event.DocumentId, @event.ProviderName, @event.Status, @event.ExternalId);

        var contract = await db.Contracts
            .Include(c => c.IntegrationStates)
            .FirstOrDefaultAsync(c => c.Id == @event.DocumentId, cancellationToken);

        if (contract is null)
        {
            logger.LogWarning(
                "Contract {ContractId} not found for ProviderStatusChanged, skipping",
                @event.DocumentId);
            return;
        }

        // Map from Contract enum to Domain enum (identical values)
        var domainStatus = (DomainSyncStatus)(int)@event.Status;

        contract.AddOrUpdateProviderState(
            @event.ProviderName,
            @event.ExternalId,
            domainStatus,
            @event.ErrorMessage);

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Updated provider state for contract {ContractId}: {Provider} → {Status}, ExternalId = {ExternalId}",
            contract.Id, @event.ProviderName, @event.Status, @event.ExternalId);
    }
}
