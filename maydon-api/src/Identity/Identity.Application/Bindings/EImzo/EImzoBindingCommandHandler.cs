using System.Text.Json;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Authentication;
using Identity.Application.Authentication.Registration.EImzo;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.EImzo;
using Identity.Application.Core.Options;
using Identity.Domain.Companies;
using Identity.Domain.IntegrationService;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Bindings.EImzo;

internal sealed class EImzoBindingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IEImzoService eImzoService,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<EImzoBindingCommand, bool>
{
	private bool isIndividual;
	public async Task<Result<bool>> Handle(EImzoBindingCommand command, CancellationToken cancellationToken)
	{
		if (!executionContextProvider.IsIndividual)
			return Result.Failure<bool>(sharedViewLocalizer.OnlyIndividualsCanBind(nameof(EImzoBindingCommand.Pkcs7)));

		// Pkcs7
		var authResult = await eImzoService.AuthAsync(command.Pkcs7, cancellationToken);
		if (authResult.IsFailure)
			return Result.Failure<bool>(authResult.Error);

		await CreateIntegrationService(JsonSerializer.Serialize(authResult.Value), IntegrationServiceType.EImzo, cancellationToken);

		isIndividual = this.IsIndividual(authResult.Value);

		var checkSerialNumberResult = await this.CheckSerialNumber(authResult.Value, cancellationToken);
		if (checkSerialNumberResult.IsFailure)
			return Result.Failure<bool>(checkSerialNumberResult.Error);

		var currentUser = await dbContext.Users.FindAsync([executionContextProvider.UserId], cancellationToken);
		if (currentUser is null)
			return Result.Failure<bool>(sharedViewLocalizer.UserNotFound(nameof(User)));

		if (command.UpdateUserData)
			currentUser = await UpdateUserAsync(authResult.Value, currentUser, cancellationToken);

		if (!isIndividual)
		{
			// create company 
			var companyResult = await CreateCompanyAsync(authResult.Value, currentUser.Id, cancellationToken);
			if (companyResult.IsFailure)
				return Result.Failure<bool>(companyResult.Error);

			var defaultRoleId = await GetOwnerRoleIdAsync(cancellationToken);

			// create company director
			await CreateCompanyUserAsync(companyResult.Value.Id, currentUser.Id, true, cancellationToken);

			await CreateAccountAsync(companyResult.Value.Id, currentUser.Id, defaultRoleId, Domain.Accounts.AccountType.Owner, cancellationToken);
		}

		return true;
	}
	private async ValueTask<Result> CheckSerialNumber(AuthResponse authResponse, CancellationToken cancellationToken)
	{
		// check serial number 
		if (string.IsNullOrWhiteSpace(authResponse.SubjectCertificateInfo.SerialNumber) || authResponse.SubjectCertificateInfo.SerialNumber.Length != EImzoRegistrationCommandHandler.EImzoSerialNumberLength)
			return Result.Failure(sharedViewLocalizer.EImzoInvalidSignature(authResponse.SubjectCertificateInfo.SerialNumber));

		// check company
		if (!isIndividual && await dbContext.Companies.AsNoTracking().AnyAsync(item => item.SerialNumber == authResponse.SubjectCertificateInfo.SerialNumber, cancellationToken))
			return Result.Failure(sharedViewLocalizer.EImzoAlreadyBound(authResponse.SubjectCertificateInfo.SerialNumber));

		// check user
		if (await dbContext.Users.AsNoTracking().AnyAsync(item => item.SerialNumber == authResponse.SubjectCertificateInfo.SerialNumber, cancellationToken))
			return Result.Failure(sharedViewLocalizer.EImzoAlreadyBound(authResponse.SubjectCertificateInfo.SerialNumber));

		return Result.Success();
	}
	private bool IsIndividual(AuthResponse auth)
	{
		if (!string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationName) &&
			string.Equals(auth.SubjectCertificateInfo.SubjectName.OrganizationName.Trim().ToLowerInvariant(), EImzoRegistrationCommandHandler.OrganizationNameKey.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
			return true;

		if (!string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationName) &&
			string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationTIN) &&
			!string.Equals(auth.SubjectCertificateInfo.SubjectName.OrganizationName.Trim().ToLowerInvariant(), EImzoRegistrationCommandHandler.OrganizationNameKey.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
			return false;

		return string.IsNullOrWhiteSpace(auth.SubjectCertificateInfo.SubjectName.OrganizationTIN);
	}
	private async ValueTask<User> UpdateUserAsync(AuthResponse authResponse, User user, CancellationToken cancellationToken)
	{
		string? middleName = null;
		if (!string.IsNullOrWhiteSpace(authResponse.SubjectCertificateInfo.SubjectName.FullName))
		{
			var fullNameArray = authResponse.SubjectCertificateInfo.SubjectName.FullName.Split(' ');
			if (fullNameArray.Length >= 3)
				middleName = string.Join(" ", fullNameArray.Select((item, index) => new { item, index }).Where(item => item.index > 1).OrderBy(item => item.index).Select(item => item.item));
		}

		user.UpdateIfEmpty(
		   authResponse.SubjectCertificateInfo.SubjectName.FirstName,
		   authResponse.SubjectCertificateInfo.SubjectName.LastName,
		   middleName,
		   tin: authResponse.SubjectCertificateInfo.SubjectName.TIN,
		   pinfl: authResponse.SubjectCertificateInfo.SubjectName.PINFL,
		   serialNumber: authResponse.SubjectCertificateInfo.SerialNumber);

		dbContext.Users.Update(user);

		return user;
	}
	private async ValueTask<Result<Company>> CreateCompanyAsync(AuthResponse authResponse, Guid ownerId, CancellationToken cancellationToken)
	{
		if (await dbContext.Companies.AsNoTracking().AnyAsync(item => item.Tin == authResponse.SubjectCertificateInfo.SubjectName.OrganizationTIN, cancellationToken))
			return Result.Failure<Company>(sharedViewLocalizer.CompanyAlreadyExists(nameof(Company)));

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
