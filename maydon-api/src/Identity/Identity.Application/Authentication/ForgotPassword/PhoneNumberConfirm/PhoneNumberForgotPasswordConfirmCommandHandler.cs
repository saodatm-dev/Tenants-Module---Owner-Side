using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Authentication.Registration.PhoneNumberConfirm;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.ForgotPassword.PhoneNumberConfirm;

internal sealed class PhoneNumberForgotPasswordConfirmCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IPasswordHasher passwordHasher,
	ITokenProvider tokenProvider,
	ICryptor cryptor,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<PhoneNumberForgotPasswordConfirmCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(PhoneNumberForgotPasswordConfirmCommand command, CancellationToken cancellationToken)
	{
		var userStateId = cryptor.DecryptUserState(command.Key);
		if (userStateId is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserStateInvalid(command.Key));

		var userState = await dbContext.UserStates.FindAsync([userStateId], cancellationToken);
		if (userState is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserStateInvalid(command.Key));

		if (!userState.IsStateActive(dateTimeProvider.UtcNow))
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserStateTimeIsExpired(nameof(PhoneNumberRegistrationConfirmCommand.Key)));

		var user = await dbContext.Users
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.Accounts)
			.FirstOrDefaultAsync(item => item.PhoneNumber == userState.PhoneNumber, cancellationToken);

		if (user is null || !user.Accounts.Any())
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.AccountNotFound(nameof(User)));

		if (!passwordHasher.TryHash(command.Password, out var passwordHash, out var salt))
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.PasswordIsEmpty(command.Password));

		dbContext.Users.Update(user.ChangePassword(passwordHash, salt));

		dbContext.UserStates.Remove(userState);

		var defaultAccount = user.Accounts
			.OrderByDescending(item => item.IsDefault)
			.ThenByDescending(item => item.Type)
			.FirstOrDefault();

		defaultAccount ??= user.Accounts.First();

		return await CreateTokenAsync(defaultAccount, cancellationToken);
	}
}
