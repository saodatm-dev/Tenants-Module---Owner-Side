using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Exceptions;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Authentication;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Users.Switch;

internal sealed class SwitchAccountCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<SwitchAccountCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(SwitchAccountCommand command, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsHost)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.AlreadyHost(nameof(SwitchAccountCommand)));

		var session = await dbContext.Sessions.AsNoTracking().FirstOrDefaultAsync(item => item.Id == executionContextProvider.SessionId, cancellationToken);
		if (session is null || session.IsTerminated)
			throw new AuthorizationException("User", "Unauthorized user");

		// is company hostable 
		if (!executionContextProvider.IsIndividual)
		{
			if (await dbContext.Companies.AsNoTracking().AnyAsync(item => item.Id == executionContextProvider.TenantId && !item.IsHost, cancellationToken))
				return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.YouAreNotUsingHost(nameof(SwitchAccountCommand)));
		}
		else
		{
			if (await dbContext.Users.AsNoTracking().AnyAsync(item => item.Id == executionContextProvider.UserId && !item.IsHost, cancellationToken))
				return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.YouAreNotUsingHost(nameof(SwitchAccountCommand)));
		}

		return await CreateTokenAsync(
			new UserAccount(
				executionContextProvider.TenantId,
				executionContextProvider.UserId,
				executionContextProvider.RoleId,
				!executionContextProvider.IsHost,
				executionContextProvider.UserName,
				executionContextProvider.CompanyName,
				executionContextProvider.SessionId),
			cancellationToken);
	}
}
