using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services;
using Core.Application.Resources;
using Core.Domain.Enums;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Commands;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Commands.Regenerate;

/// <summary>
/// Regenerates a contract body in Draft status.
/// Increments the version and publishes a snapshot to the versioning system.
/// </summary>
public sealed class RegenerateContractCommandHandler(
    IDocumentDbContext dbContext,
    ISharedViewLocalizer sharedViewLocalizer,
    IVersioningService versioningService)
    : ICommandHandler<RegenerateContractCommand>
{
    public async Task<Result> Handle(
        RegenerateContractCommand command,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts
            .FirstOrDefaultAsync(c => c.Id == command.ContractId, cancellationToken);

        if (contract is null)
            return Result.Failure(sharedViewLocalizer.ContractNotFound(nameof(command.ContractId)));


        contract.Regenerate(command.Body);

        await dbContext.SaveChangesAsync(cancellationToken);

        await versioningService.PublishVersionSnapshotAsync(contract, EntityChangeType.Updated, cancellationToken);

        return Result.Success();
    }
}
