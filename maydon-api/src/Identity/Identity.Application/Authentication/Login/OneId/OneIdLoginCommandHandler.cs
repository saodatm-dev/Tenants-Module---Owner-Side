using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.OneId;
using Identity.Application.Core.Options;
using Identity.Domain.Accounts;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Login.OneId;

internal sealed class OneIdLoginCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IOneIdService oneIdService,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<OneIdLoginCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(OneIdLoginCommand command, CancellationToken cancellationToken)
	{
		var accessTokenResponse = await oneIdService.AuthorizationAsync(command.Code, cancellationToken);
		if (accessTokenResponse.IsFailure)
			return Result.Failure<AuthenticationResponse>(accessTokenResponse.Error);

		var oneIdResult = await oneIdService.GetAsync(accessTokenResponse.Value.AccessToken, cancellationToken);
		if (oneIdResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(oneIdResult.Error);

		var user = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.Pinfl == oneIdResult.Value.Pinfl, cancellationToken);

		if (user is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserNotFound(nameof(User)));

		var accounts = await dbContext.Accounts
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => item.UserId == user.Id)
			.Include(item => item.Company)
			.ToListAsync(cancellationToken);

		if (!accounts.Any())
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.NotFound(nameof(User)));

		var accountType = (OneIdUserType)oneIdResult.Value.UserType == OneIdUserType.Individual
			? AccountType.Client
			: AccountType.Owner;

		var account = accounts.FirstOrDefault(item => item.Type == accountType);
		account ??= accounts.FirstOrDefault(item => item.IsDefault);
		account ??= accounts.First();

		return await CreateTokenAsync(account, cancellationToken);
	}
}
