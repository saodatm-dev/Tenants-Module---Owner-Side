using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services;
using Core.Application.Resources;
using Core.Domain.Enums;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Commands;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Commands.UpdateBody;

/// <summary>
/// Updates the JSONB body of a contract.
/// Only allowed when the contract is in Draft status.
/// </summary>
public sealed class UpdateContractBodyCommandHandler(
    IDocumentDbContext dbContext,
    ISharedViewLocalizer sharedViewLocalizer,
    IVersioningService versioningService)
    : ICommandHandler<UpdateContractBodyCommand>
{
    public async Task<Result> Handle(
        UpdateContractBodyCommand command,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts
            .FirstOrDefaultAsync(c => c.Id == command.ContractId, cancellationToken);

        if (contract is null)
            return Result.Failure(
                sharedViewLocalizer.ContractNotFound(nameof(command.ContractId)));



        contract.UpdateBody(command.Body);

        await dbContext.SaveChangesAsync(cancellationToken);

        await versioningService.PublishVersionSnapshotAsync(contract, EntityChangeType.Updated, cancellationToken);

        return Result.Success();
    }
}
