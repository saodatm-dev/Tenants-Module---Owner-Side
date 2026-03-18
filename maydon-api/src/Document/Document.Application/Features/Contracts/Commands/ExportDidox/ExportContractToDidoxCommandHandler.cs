using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services;
using Core.Application.Resources;
using Core.Domain.Enums;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Commands;
using Document.Contract.IntegrationEvents;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Commands.ExportDidox;

/// <summary>
/// Exports a contract to Didox for signing.
/// Transitions the contract from Draft to PendingSignature, locks body edits,
/// and publishes an integration event for background PDF generation + Didox upload.
/// </summary>
public sealed class ExportContractToDidoxCommandHandler(
    IDocumentDbContext dbContext,
    IIntegrationEventPublisher eventPublisher,
    IExecutionContextProvider executionContext,
    ISharedViewLocalizer sharedViewLocalizer,
    IVersioningService versioningService) : ICommandHandler<ExportContractToDidoxCommand>
{
    public async Task<Result> Handle(
        ExportContractToDidoxCommand command,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts
            .Include(c => c.IntegrationStates)
            .FirstOrDefaultAsync(c => c.Id == command.ContractId, cancellationToken);

        if (contract is null)
            return Result.Failure(
                sharedViewLocalizer.ContractNotFound(nameof(command.ContractId)));


        contract.ExportToDidox();

        await dbContext.SaveChangesAsync(cancellationToken);

        var userId = executionContext.IsAuthorized ? executionContext.UserId : (Guid?)null;
        await eventPublisher.PublishAsync(new InitiateDocumentExport(contract.Id, "Didox", userId), cancellationToken);
        await versioningService.PublishVersionSnapshotAsync(contract, EntityChangeType.Updated, cancellationToken);

        return Result.Success();
    }
}
