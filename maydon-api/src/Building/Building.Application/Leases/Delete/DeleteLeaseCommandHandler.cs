using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Leases.Delete;

internal sealed class DeleteLeaseCommandHandler(
    ISharedViewLocalizer sharedViewLocalizer,
    IBuildingDbContext dbContext) : ICommandHandler<DeleteLeaseCommand>
{
    public async Task<Result> Handle(DeleteLeaseCommand command, CancellationToken cancellationToken)
    {
        var maybeItem = await dbContext.Leases
            .FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

        if (maybeItem is null)
            return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(DeleteLeaseCommand.Id)));

        dbContext.Leases.Remove(maybeItem.Remove());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
