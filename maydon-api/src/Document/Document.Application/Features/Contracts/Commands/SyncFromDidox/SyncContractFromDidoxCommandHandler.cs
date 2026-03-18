using Core.Application.Abstractions.Services;
using Core.Application.Resources;
using Core.Domain.Enums;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Commands;
using Document.Contract.Gateways;
using Document.Domain.Contracts.Enums;
using Document.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Core.Application.Abstractions.Messaging;

namespace Document.Application.Features.Contracts.Commands.SyncFromDidox;

/// <summary>
/// Handles <see cref="SyncContractFromDidoxCommand"/>.
/// Polls the Didox API for the contract's current envelope status,
/// reconciles with local state, and publishes a version snapshot.
/// </summary>
public sealed class SyncContractFromDidoxCommandHandler(
    IDocumentDbContext dbContext,
    IDocumentProviderService providerService,
    ISharedViewLocalizer sharedViewLocalizer,
    IVersioningService versioningService,
    ILogger<SyncContractFromDidoxCommandHandler> logger) : ICommandHandler<SyncContractFromDidoxCommand>
{
    public async Task<Result> Handle(
        SyncContractFromDidoxCommand command,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts
            .Include(c => c.IntegrationStates)
            .FirstOrDefaultAsync(c => c.Id == command.ContractId, cancellationToken);

        if (contract is null)
            return Result.Failure(
                sharedViewLocalizer.ContractNotFound(nameof(command.ContractId)));

        // Find the Didox provider state
        var didoxState = contract.IntegrationStates
            .FirstOrDefault(s => s.ProviderName == "Didox");

        if (didoxState is null || string.IsNullOrEmpty(didoxState.ExternalId))
            return Result.Failure(
                sharedViewLocalizer.ContractNoDidoxState(nameof(command.ContractId)));

        // Fetch current status from Didox
        var statusResult = await providerService.GetDocumentStatusAsync(
            didoxState.ExternalId,
            "Didox",
            contract.CreatedByUserId,
            cancellationToken);

        if (statusResult.IsFailure)
        {
            logger.LogError("Failed to fetch Didox status for contract {ContractId}: {Error}",
                contract.Id, statusResult.Error.Description);
            return Result.Failure(statusResult.Error);
        }

        var providerStatus = statusResult.Value;

        // Reconcile: map Didox status → Contract state
        if (providerStatus.IsSigned)
        {
            var signedAt = providerStatus.SignedAt ?? DateTime.UtcNow;

            if (contract.OwnerSignedAt is null)
            {
                contract.RecordOwnerSigned(signedAt);
                logger.LogInformation("Contract {ContractId} — owner signed at {SignedAt}", contract.Id, signedAt);
            }
            else if (contract.ClientSignedAt is null)
            {
                contract.RecordClientSigned(signedAt);
                logger.LogInformation("Contract {ContractId} — client signed at {SignedAt}", contract.Id, signedAt);
            }

            contract.AddOrUpdateProviderState("Didox", didoxState.ExternalId,
                ExternalSyncStatus.Signed);
        }
        else if (providerStatus.IsRejected)
        {
            contract.RecordRejected(
                SigningParty.Client,
                DateTime.UtcNow,
                providerStatus.RejectionReason);

            contract.AddOrUpdateProviderState("Didox", didoxState.ExternalId,
                ExternalSyncStatus.Rejected, providerStatus.RejectionReason);

            logger.LogInformation("Contract {ContractId} — rejected by Didox: {Reason}",
                contract.Id, providerStatus.RejectionReason);
        }
        else
        {
            logger.LogInformation("Contract {ContractId} — Didox status unchanged: {Status}",
                contract.Id, providerStatus.RawStatus);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await versioningService.PublishVersionSnapshotAsync(contract, EntityChangeType.Updated, cancellationToken);

        return Result.Success();
    }
}
