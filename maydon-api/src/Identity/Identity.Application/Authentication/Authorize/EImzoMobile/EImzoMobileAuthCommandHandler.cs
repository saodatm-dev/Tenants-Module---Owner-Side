using System.Text.Json;
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
using Identity.Domain.Companies;
using Identity.Domain.IntegrationService;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Authorize.EImzoMobile;

internal sealed class EImzoMobileAuthCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IEImzoService eImzoService,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<EImzoMobileAuthCommand, AuthenticationResponse>
{
	internal const short EImzoSerialNumberLength = 8;
	internal const string OrganizationNameKey = "НЕ УКАЗАНО";
	internal const string OrganizationNameEntrepreneur = "Якка тартибдаги тадбиркор";

	public async Task<Result<AuthenticationResponse>> Handle(EImzoMobileAuthCommand command, CancellationToken cancellationToken)
	{
		var authResult = await eImzoService.MobileAuthenticateAsync(command.DocumentId, cancellationToken);
		if (authResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(authResult.Error);

		var existingUser = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.SerialNumber == authResult.Value.SubjectCertificateInfo.SerialNumber, cancellationToken);

		if (existingUser is not null)
		{
			var accounts = await dbContext.Accounts
				.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
				.Where(item => item.UserId == existingUser.Id)
				.ToListAsync(cancellationToken);

			if (!accounts.Any())
				return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.NotFound(nameof(command.DocumentId)));

			var isIndividualUser = IsIndividual(authResult.Value);
			var accountType = isIndividualUser
				? Domain.Accounts.AccountType.Client
				: Domain.Accounts.AccountType.Owner;

			var account = accounts.FirstOrDefault(item => item.Type == accountType);
			account ??= accounts.FirstOrDefault(item => item.IsDefault);
			account ??= accounts.First();

			return await CreateTokenAsync(account, cancellationToken);
		}

		await CreateIntegrationService(JsonSerializer.Serialize(authResult.Value), IntegrationServiceType.EImzo, cancellationToken);

		var isIndividual = IsIndividual(authResult.Value);

		var validationResult = await Validate(authResult.Value, isIndividual, cancellationToken);
		if (validationResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(validationResult.Error);

		var defaultRoleId = isIndividual
			? await GetClientRoleIdAsync(cancellationToken)
			: await GetOwnerRoleIdAsync(cancellationToken);

		var userResult = await CreateUserAsync(authResult.Value, cancellationToken);
		if (userResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(userResult.Error);

		var tenantId = userResult.Value.Id;
		if (!isIndividual)
		{
			var companyResult = await CreateCompanyAsync(authResult.Value, userResult.Value.Id, cancellationToken);
			if (companyResult.IsFailure)
				return Result.Failure<AuthenticationResponse>(companyResult.Error);

			await CreateCompanyUserAsync(companyResult.Value.Id, userResult.Value.Id, true, cancellationToken);

			tenantId = companyResult.Value.Id;
		}

		return await CreateTokenAsync(
			await CreateAccountAsync(
				tenantId,
				userResult.Value.Id,
				defaultRoleId,
				isIndividual ? Domain.Accounts.AccountType.Client : Domain.Accounts.AccountType.Owner,
				cancellationToken),
			cancellationToken);
	}

	private async ValueTask<Result> Validate(AuthResponse authResponse, bool isIndividual, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(authResponse.SubjectCertificateInfo.SerialNumber) || authResponse.SubjectCertificateInfo.SerialNumber.Length != EImzoSerialNumberLength)
			return Result.Failure(sharedViewLocalizer.InvalidValue(authResponse.SubjectCertificateInfo.SerialNumber));

		if (!isIndividual &&
		   (await dbContext.Companies.AsNoTracking().AnyAsync(item => item.SerialNumber == authResponse.SubjectCertificateInfo.SerialNumber || item.Tin == authResponse.SubjectCertificateInfo.SubjectName.OrganizationTIN, cancellationToken)))
			return Result.Failure(sharedViewLocalizer.AlreadyExists(authResponse.SubjectCertificateInfo.SerialNumber));

		if (await dbContext.Users.AsNoTracking().AnyAsync(item => item.SerialNumber == authResponse.SubjectCertificateInfo.SerialNumber || item.Pinfl == authResponse.SubjectCertificateInfo.SubjectName.PINFL, cancellationToken))
			return Result.Failure(sharedViewLocalizer.AlreadyExists(authResponse.SubjectCertificateInfo.SerialNumber));

		return Result.Success();
	}

	private bool IsIndividual(AuthResponse auth)
	{
		if (!string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationName) &&
			string.Equals(auth.SubjectCertificateInfo.SubjectName.OrganizationName.Trim().ToLowerInvariant(), OrganizationNameKey.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
			return true;

		if (!string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationName) &&
			string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationTIN) &&
			!string.Equals(auth.SubjectCertificateInfo.SubjectName.OrganizationName.Trim().ToLowerInvariant(), OrganizationNameKey.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
			return false;

		return string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationTIN);
	}

	private async ValueTask<Result<User>> CreateUserAsync(AuthResponse authResponse, CancellationToken cancellationToken)
	{
		string? middleName = null;
		if (!string.IsNullOrWhiteSpace(authResponse.SubjectCertificateInfo.SubjectName.FullName))
		{
			var fullNameArray = authResponse.SubjectCertificateInfo.SubjectName.FullName.Split(' ');
			if (fullNameArray.Length >= 3)
				middleName = string.Join(" ", fullNameArray.Select((item, index) => new { item, index }).Where(item => item.index > 1).OrderBy(item => item.index).Select(item => item.item));
		}

		var user = new User(
			RegisterType.EImzo,
			authResponse.SubjectCertificateInfo.SubjectName.FirstName,
			authResponse.SubjectCertificateInfo.SubjectName.LastName,
			middleName,
			tin: authResponse.SubjectCertificateInfo.SubjectName.TIN,
			pinfl: authResponse.SubjectCertificateInfo.SubjectName.PINFL,
			serialNumber: authResponse.SubjectCertificateInfo.SerialNumber);

		await dbContext.Users.AddAsync(user, cancellationToken);

		return user;
	}

	private async ValueTask<Result<Company>> CreateCompanyAsync(AuthResponse authResponse, Guid ownerId, CancellationToken cancellationToken)
	{
		var company = new Company(
			ownerId,
			authResponse.SubjectCertificateInfo.SubjectName.OrganizationName,
			RegisterType.EImzo,
			tin: authResponse.SubjectCertificateInfo.SubjectName.OrganizationTIN,
			serialNumber: authResponse.SubjectCertificateInfo.SerialNumber,
			isVerified: true);

		await dbContext.Companies.AddAsync(company, cancellationToken);

		return company;
	}
}
