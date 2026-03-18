using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
	{
		var session = await dbContext.Sessions
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.Account)
			.AsSplitQuery()
			.FirstOrDefaultAsync(item =>
				!item.IsTerminated &&
				item.RefreshToken == command.RefreshToken, cancellationToken);

		if (session is null || session.Account is null || session.IsExpired(dateTimeProvider.UtcNow))
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.NotFound(nameof(RefreshTokenCommand.RefreshToken)));

		return await CreateTokenAsync(
			session.Account,
			cancellationToken,
			session.Id);
	}
}
