using System.Text.Json;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.OneId;
using Identity.Application.Core.Options;
using Identity.Domain.Companies;
using Identity.Domain.IntegrationService;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Registration.OneId;

internal sealed class OneIdRegistrationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IOneIdService oneIdService,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<OneIdRegistrationCommand, AuthenticationResponse>
{
	private bool isIndividual;
	private Guid tenantId = Guid.Empty;
	public async Task<Result<AuthenticationResponse>> Handle(OneIdRegistrationCommand command, CancellationToken cancellationToken)
	{
		// authorization 
		var accessTokenResponse = await oneIdService.AuthorizationAsync(command.Code, cancellationToken);
		if (accessTokenResponse.IsFailure)
			return Result.Failure<AuthenticationResponse>(accessTokenResponse.Error);

		var oneIdResult = await oneIdService.GetAsync(accessTokenResponse.Value.AccessToken, cancellationToken);
		if (oneIdResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(oneIdResult.Error);

		await CreateIntegrationService(JsonSerializer.Serialize(oneIdResult.Value), IntegrationServiceType.OneID, cancellationToken);

		var validationResult = await this.Validate(oneIdResult.Value, cancellationToken);
		if (validationResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(validationResult.Error);

		isIndividual = (OneIdUserType)oneIdResult.Value.UserType == OneIdUserType.Individual;

		var defaultRoleId = isIndividual
			? await GetClientRoleIdAsync(cancellationToken)
			: await GetOwnerRoleIdAsync(cancellationToken);

		var userResult = await CreateUserAsync(oneIdResult.Value, defaultRoleId, cancellationToken);
		if (userResult.IsFailure)
			return Result.Failure<AuthenticationResponse>(userResult.Error);

		tenantId = userResult.Value.Id;
		if (!isIndividual)
		{
			// create company
			var companyResult = await CreateCompanyAsync(oneIdResult.Value, userResult.Value.Id, cancellationToken);
			if (companyResult != Result<Company>.None && companyResult.IsFailure)
				return Result.Failure<AuthenticationResponse>(companyResult.Error);

			if (companyResult.IsSuccess)
			{
				await CreateCompanyUserAsync(companyResult.Value.Id, userResult.Value.Id, true, cancellationToken);

				tenantId = companyResult.Value.Id;
			}
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
	private async ValueTask<Result> Validate(OneIdResponse oneIdResponse, CancellationToken cancellationToken)
	{
		var companyInfo = oneIdResponse.CompanyInfo?.FirstOrDefault(item => item.IsBasic);
		if (companyInfo is not null)
		{
			// check companyResult
			if (!isIndividual &&
			   (await dbContext.Companies.AsNoTracking().AnyAsync(item => item.Tin == companyInfo.CompanyTin, cancellationToken)))
				return Result.Failure(sharedViewLocalizer.AlreadyExists(companyInfo.CompanyTin));
		}

		// check user
		if (await dbContext.Users.AsNoTracking().AnyAsync(item => item.Pinfl == oneIdResponse.Pinfl, cancellationToken))
			return Result.Failure(sharedViewLocalizer.AlreadyExists(oneIdResponse.Pinfl));

		return Result.Success();
	}

	private async ValueTask<Result<User>> CreateUserAsync(OneIdResponse response, Guid roleId, CancellationToken cancellationToken)
	{
		var companyInfo = response.CompanyInfo?.FirstOrDefault(item => item.IsBasic);

		var user = new User(
			RegisterType.OneID,
			response.FirstName,
			response.LastName,
			response.MiddleName,
			tin: companyInfo?.Tin,
			pinfl: response.Pinfl,
			passportNumber: response.PasportNumber,
			birthDate: response.BirthDate);

		await dbContext.Users.AddAsync(user, cancellationToken);

		return user;
	}
	private async ValueTask<Result<Company>> CreateCompanyAsync(OneIdResponse response, Guid ownerId, CancellationToken cancellationToken)
	{
		var companyInfo = response.CompanyInfo?.FirstOrDefault(item => item.IsBasic);
		if (companyInfo is null)
			return Result<Company>.None;

		if (await dbContext.Companies.AsNoTracking().AnyAsync(item => item.Tin == companyInfo.CompanyTin, cancellationToken))
			return Result.Failure<Company>(sharedViewLocalizer.AlreadyExists(nameof(Company)));

		var company = new Company(
				ownerId,
				companyInfo.CompanyName,
				RegisterType.OneID,
				tin: companyInfo.CompanyTin,
				isVerified: bool.Parse(response.IsValid));

		await dbContext.Companies.AddAsync(company, cancellationToken);

		return company;
	}
}
