using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.EImzo;
using Identity.Application.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Login.EImzoMobile;

internal sealed class EImzoMobileLoginCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IEImzoService eImzoService,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<EImzoMobileLoginCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(EImzoMobileLoginCommand command, CancellationToken cancellationToken)
	{
		var authResult = await eImzoService.MobileAuthenticateAsync(command.DocumentId, cancellationToken);
		if (authResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(authResult.Error);

		var user = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.SerialNumber == authResult.Value.SubjectCertificateInfo.SerialNumber, cancellationToken);

		if (user is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserNotFound(nameof(command.DocumentId)));

		var accounts = await dbContext.Accounts
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => item.UserId == user.Id)
			.ToListAsync(cancellationToken);

		if (!accounts.Any())
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.NotFound(nameof(command.DocumentId)));

		var account = accounts.FirstOrDefault(item => item.IsDefault);
		account ??= accounts.First();

		return await CreateTokenAsync(account, cancellationToken);
	}
}
