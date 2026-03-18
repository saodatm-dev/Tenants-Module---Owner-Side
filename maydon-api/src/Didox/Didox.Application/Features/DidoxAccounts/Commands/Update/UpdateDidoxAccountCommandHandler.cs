using Core.Domain.Results;
using Core.Application.Resources;
using Core.Application.Abstractions.Security;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Abstractions.Database;
using Didox.Application.Contracts.DidoxAccounts.Commands;
using Microsoft.EntityFrameworkCore;

namespace Didox.Application.Features.DidoxAccounts.Commands.Update;

public class UpdateDidoxAccountCommandHandler(
    IDidoxDbContext dbContext,
    IStringEncryptor encryptor,
    ISharedViewLocalizer sharedViewLocalizer,
    IExecutionContextProvider executionContextProvider)
    : ICommandHandler<UpdateDidoxAccountCommand>
{
    public async Task<Result> Handle(UpdateDidoxAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == command.Id && !a.IsDeleted, cancellationToken);

        if (account is null)
        {
            return Result.Failure(sharedViewLocalizer.ResourceNotFound("Account", nameof(UpdateDidoxAccountCommand.Id)));
        }

        if (!string.IsNullOrWhiteSpace(command.Login) && command.Login != account.Login)
        {
            account.Login = command.Login;
        }

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            account.Password = encryptor.Encrypt(command.Password);
        }

        account.UpdatedAt = DateTime.UtcNow;
        account.UpdatedBy = executionContextProvider.UserId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
