using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Login.PhoneNumber;

internal sealed class PhoneNumberLoginCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IPasswordHasher passwordHasher,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<PhoneNumberLoginCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(PhoneNumberLoginCommand command, CancellationToken cancellationToken)
	{
		// clear phone number
		var phoneNumber = command.PhoneNumber.Replace('+', ' ').Trim();

		var user = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.PhoneNumber == phoneNumber, cancellationToken);

		if (user is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserNotFound(command.PhoneNumber));

		if (!passwordHasher.Verify(command.Password, user.Password, user.Salt))
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.PhoneNumberOrPasswordIncorrect(command.PhoneNumber));

		var accounts = await dbContext.Accounts
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => item.UserId == user.Id)
			.ToListAsync(cancellationToken);

		if (!accounts.Any())
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.AccountNotFound(nameof(User)));

		var account = accounts.FirstOrDefault(item => item.IsDefault);
		account ??= accounts.First();

		return await CreateTokenAsync(account, cancellationToken);
	}
}
