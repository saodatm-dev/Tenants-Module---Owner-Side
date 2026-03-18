using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Registration.PhoneNumberConfirm;

internal sealed class PhoneNumberRegistrationConfirmCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IPasswordHasher passwordHasher,
	ITokenProvider tokenProvider,
	ICryptor cryptor,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<PhoneNumberRegistrationConfirmCommand, AuthenticationResponse>
{
	public async Task<Result<AuthenticationResponse>> Handle(PhoneNumberRegistrationConfirmCommand command, CancellationToken cancellationToken)
	{
		var userStateId = cryptor.DecryptUserState(command.Key);
		if (userStateId is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.InvalidValue(command.Key));

		var userState = await dbContext.UserStates.FindAsync([userStateId], cancellationToken);
		if (userState is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.InvalidValue(command.Key));

		if (!userState.IsStateActive(dateTimeProvider.UtcNow))
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.UserStateTimeIsExpired(nameof(PhoneNumberRegistrationConfirmCommand.Key)));

		var user = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.PhoneNumber == userState.PhoneNumber, cancellationToken);

		if (user is not null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.AlreadyExists(nameof(User)));

		var roleId = await GetClientRoleIdAsync(cancellationToken);

		var userResult = await CreateUserAsync(command, userState.PhoneNumber, roleId, cancellationToken);
		if (userResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(userResult.Error);

		dbContext.UserStates.Remove(userState);

		return await CreateTokenAsync(
			await CreateAccountAsync(
				userResult.Value.Id,
				userResult.Value.Id,
				roleId,
				Domain.Accounts.AccountType.Client,
				cancellationToken),
			cancellationToken);
	}
	private async ValueTask<Result<User>> CreateUserAsync(PhoneNumberRegistrationConfirmCommand command, string phoneNumber, Guid roleId, CancellationToken cancellationToken)
	{
		if (!passwordHasher.TryHash(command.Password, out var passwordHash, out var salt))
			return Result.Failure<User>(sharedViewLocalizer.PasswordIsEmpty(command.Password));

		var user = new User(
			RegisterType.PhoneNumber,
			phoneNumber: phoneNumber,
			password: passwordHash,
			salt: salt);

		await dbContext.Users.AddAsync(user, cancellationToken);

		return user;
	}
}
