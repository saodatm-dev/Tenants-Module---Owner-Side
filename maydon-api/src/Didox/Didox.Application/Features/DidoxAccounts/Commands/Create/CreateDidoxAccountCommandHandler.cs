using Core.Application.Resources;
using Core.Application.Abstractions.Security;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Didox.Application.Abstractions.Database;
using Didox.Application.Contracts.DidoxAccounts.Commands;
using Didox.Domain.Entities;

using Didox.Application.Contracts.DidoxAccounts.Responses;
using Didox.Application.Mappings;

namespace Didox.Application.Features.DidoxAccounts.Commands.Create;

public class CreateDidoxAccountCommandHandler(
    IDidoxDbContext dbContext,
    ISharedViewLocalizer localizer,
    IExecutionContextProvider executionContextProvider,
    IStringEncryptor encryptor) : ICommandHandler<CreateDidoxAccountCommand, DidoxAccountResponse>
{
    public async Task<Result<DidoxAccountResponse>> Handle(CreateDidoxAccountCommand command, CancellationToken cancellationToken = default)
    {
        if (!executionContextProvider.IsAuthorized)
        {
            return Result.Failure<DidoxAccountResponse>(localizer.UnAuthorizedAccess(nameof(CreateDidoxAccountCommand)));
        }

        var ownerId = executionContextProvider.UserId;
        var isPinfl = command.Login.Length == 14;
        var account = new DidoxAccount
        {
            Id = Guid.NewGuid(),
            Login = command.Login,
            OwnerId = ownerId,
            CreatedDate = DateTime.UtcNow,
            IsDeleted = false,
            Pinfl = isPinfl ? command.Login : null,
            Tin = !isPinfl ? command.Login : null,
            Password = encryptor.Encrypt(command.Password)
        };

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(account.ToResponse());
    }
}

