using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Authentication;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Accounts.CreateOwner;

internal sealed class CreateOwnerAccountCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<CreateOwnerAccountCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(CreateOwnerAccountCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Accounts
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.Type == AccountType.Owner, cancellationToken);

		if (maybeItem is not null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.AlreadyExists(nameof(Account)));

		var roleId = await GetOwnerRoleIdAsync(cancellationToken);

		return await CreateTokenAsync(
			await CreateAccountAsync(
				executionContextProvider.TenantId,
				executionContextProvider.UserId,
				roleId,
				AccountType.Owner,
				cancellationToken: cancellationToken),
			cancellationToken);
	}
}
