using System.Text.Json;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Authentication;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.OneId;
using Identity.Application.Core.Options;
using Identity.Domain.Companies;
using Identity.Domain.IntegrationService;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Bindings.OneId;

internal sealed class OneIdBindingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IDateTimeProvider dateTimeProvider,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IOneIdService oneIdService,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
	: BaseAuthenticationCommandHandler(executionContextProvider, dateTimeProvider, dbContext, tokenProvider, options),
	  ICommandHandler<OneIdBindingCommand, bool>
{
	public async Task<Result<bool>> Handle(OneIdBindingCommand command, CancellationToken cancellationToken)
	{
		// authorization 
		var accessTokenResponse = await oneIdService.AuthorizationAsync(command.Code, cancellationToken);
		if (accessTokenResponse.IsFailure)
			return Result.Failure<bool>(accessTokenResponse.Error);

		var oneIdResult = await oneIdService.GetAsync(accessTokenResponse.Value.AccessToken, cancellationToken);
		if (oneIdResult.IsFailure)
			return Result.Failure<bool>(oneIdResult.Error);

		await CreateIntegrationService(JsonSerializer.Serialize(oneIdResult.Value), IntegrationServiceType.OneID, cancellationToken);

		var currentUser = await dbContext.Users.FindAsync([executionContextProvider.UserId], cancellationToken);
		if (currentUser is null)
			return Result.Failure<bool>(sharedViewLocalizer.UserNotFound(nameof(User)));

		var user = UpdateUserAsync(oneIdResult.Value, currentUser, cancellationToken);

		if ((OneIdUserType)oneIdResult.Value.UserType != OneIdUserType.Individual)
		{
			// create company 
			var companyResult = await CreateCompanyAsync(oneIdResult.Value, currentUser.Id, cancellationToken);
			if (companyResult != Result<Company>.None && companyResult.IsFailure)
				return Result.Failure<bool>(companyResult.Error);

			if (companyResult.IsSuccess)
			{
				var defaultRoleId = await GetOwnerRoleIdAsync(cancellationToken);

				await CreateCompanyUserAsync(companyResult.Value.Id, currentUser.Id, true, cancellationToken);

				await CreateAccountAsync(companyResult.Value.Id, currentUser.Id, defaultRoleId, Domain.Accounts.AccountType.Owner, cancellationToken);
			}
		}

		return true;
	}
	private User UpdateUserAsync(OneIdResponse response, User user, CancellationToken cancellationToken)
	{
		var companyInfo = response.CompanyInfo?.FirstOrDefault(item => item.IsBasic);

		user.UpdateIfEmpty(
		   response.FirstName,
		   response.LastName,
		   response.MiddleName,
		   companyInfo?.Tin,
		   response.Pinfl,
		   passportNumber: response.PasportNumber,
		   birthDate: response.BirthDate);

		dbContext.Users.Update(user);

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
