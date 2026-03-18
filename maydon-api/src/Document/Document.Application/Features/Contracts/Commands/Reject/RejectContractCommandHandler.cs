using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services;
using Core.Application.Resources;
using Core.Domain.Enums;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Commands;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Commands.Reject;

/// <summary>
/// Rejects a contract by the specified party (owner or client).
/// </summary>
public sealed class RejectContractCommandHandler(
    IDocumentDbContext dbContext,
    ISharedViewLocalizer sharedViewLocalizer,
    IVersioningService versioningService)
    : ICommandHandler<RejectContractCommand>
{
    public async Task<Result> Handle(
        RejectContractCommand command,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts
            .Include(c => c.SigningEvents)
            .FirstOrDefaultAsync(c => c.Id == command.ContractId, cancellationToken);

        if (contract is null)
            return Result.Failure(
                sharedViewLocalizer.ContractNotFound(nameof(command.ContractId)));



        var party = command.Party.ToLowerInvariant() switch
        {
            "owner" => SigningParty.Owner,
            "client" => SigningParty.Client,
            _ => (SigningParty?)null
        };

        if (party is null)
            return Result.Failure(
                sharedViewLocalizer.ContractInvalidParty(nameof(command.Party)));

        contract.RecordRejected(party.Value, DateTime.UtcNow, command.Reason);

        await dbContext.SaveChangesAsync(cancellationToken);

        await versioningService.PublishVersionSnapshotAsync(contract, EntityChangeType.Updated, cancellationToken);

        return Result.Success();
    }
}
