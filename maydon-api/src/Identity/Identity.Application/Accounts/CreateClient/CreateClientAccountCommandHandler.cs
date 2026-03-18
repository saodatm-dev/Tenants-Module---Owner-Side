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

namespace Identity.Application.Accounts.CreateClient;

internal sealed class CreateClientAccountCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<CreateClientAccountCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(CreateClientAccountCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Accounts
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.Type == AccountType.Client, cancellationToken);

		if (maybeItem is not null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.AlreadyExists(nameof(Account)));

		var roleId = await GetClientRoleIdAsync(cancellationToken);

		return await CreateTokenAsync(
			await CreateAccountAsync(
				executionContextProvider.TenantId,
				executionContextProvider.UserId,
				roleId,
				AccountType.Client,
				cancellationToken: cancellationToken),
			cancellationToken);
	}
}
