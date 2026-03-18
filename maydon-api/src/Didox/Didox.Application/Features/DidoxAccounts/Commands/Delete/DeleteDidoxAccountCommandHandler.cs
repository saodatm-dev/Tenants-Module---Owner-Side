using Core.Domain.Results;
using Core.Domain.Extensions;
using Core.Application.Resources;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Abstractions.Database;
using Didox.Application.Contracts.DidoxAccounts.Commands;
using Microsoft.EntityFrameworkCore;

namespace Didox.Application.Features.DidoxAccounts.Commands.Delete;

public class DeleteDidoxAccountCommandHandler(
    IDidoxDbContext dbContext,
    ISharedViewLocalizer sharedViewLocalizer,
    IExecutionContextProvider executionContextProvider)
    : ICommandHandler<DeleteDidoxAccountCommand>
{
    public async Task<Result> Handle(DeleteDidoxAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == command.Id && !a.IsDeleted, cancellationToken);

        if (account is null)
        {
            return Result.Failure(sharedViewLocalizer.ResourceNotFound("Account", nameof(DeleteDidoxAccountCommand.Id)));
        }

        account.SoftDelete(executionContextProvider.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


