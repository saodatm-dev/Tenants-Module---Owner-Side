using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Authentication;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Accounts.Change;

internal sealed class ChangeAccountCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ITokenProvider tokenProvider,
	ICryptor cryptor,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<ChangeAccountCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(ChangeAccountCommand command, CancellationToken cancellationToken)
	{
		var accountSession = cryptor.DecryptAccount(command.Key);
		if (accountSession is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.InvalidValue(nameof(ChangeAccountCommand.Key)));

		if (executionContextProvider.SessionId != accountSession.Value.SessionId)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.InvalidValue(nameof(ChangeAccountCommand.Key)));

		var account = await dbContext.Accounts
				.AsNoTracking()
				.FirstOrDefaultAsync(item => item.Id == accountSession.Value.AccountId, cancellationToken);

		if (account is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserNotFound(command.Key));

		return await CreateTokenAsync(
			account,
			cancellationToken);
	}
}
