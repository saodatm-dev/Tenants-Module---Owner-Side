using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Domain.Providers;
using Core.Domain.Results;
using Core.Domain.Roles;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.Accounts;
using Identity.Domain.CompanyUsers;
using Identity.Domain.IntegrationService;
using Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication;

internal class BaseAuthenticationCommandHandler(
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ITokenProvider tokenProvider,
	IOptions<ApplicationOptions> options)
{
	protected Task<Guid> GetOwnerRoleIdAsync(CancellationToken cancellationToken) => dbContext.Roles
		.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
		.Where(item => item.TenantId == null && item.Type == RoleType.Owner)
		.Select(item => item.Id)
		.FirstOrDefaultAsync(cancellationToken);
	protected Task<Guid> GetClientRoleIdAsync(CancellationToken cancellationToken) => dbContext.Roles
		.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
		.Where(item => item.TenantId == null && item.Type == RoleType.Client)
		.Select(item => item.Id)
		.FirstOrDefaultAsync(cancellationToken);

	protected async ValueTask<Account> CreateAccountAsync(Guid tenantId, Guid userId, Guid roleId, AccountType accountType, CancellationToken cancellationToken = default)
	{
		var account = new Account(
				tenantId,
				userId,
				accountType)
			.ChangeRole(roleId);

		await dbContext.Accounts.AddAsync(account, cancellationToken);

		return account;
	}
	protected async ValueTask<Result<AuthenticationResponse>> CreateTokenAsync(
		Account account,
		CancellationToken cancellationToken = default,
		Guid? sessionId = null)
	{
		var session = await UpsertSessionAsync(account, cancellationToken, sessionId);

		var roleType = await dbContext.Roles
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => item.Id == account.RoleId)
			.Select(item => item.Type)
			.FirstOrDefaultAsync(cancellationToken);

		var token = tokenProvider.Create(account, session.Id, roleType);

		var result = new AuthenticationResponse(
			token.Item1,
			token.Item2,
			session.RefreshToken,
			session.RefreshTokenExpiryTime,
			account.Type);

		await dbContext.SaveChangesAsync(cancellationToken);

		return result;
	}
	private async ValueTask<Session> UpsertSessionAsync(
		Account account,
		CancellationToken cancellationToken,
		Guid? sessionId = null)
	{
		Session? maybeItem = null;

		if (sessionId.HasValue)
		{
			maybeItem = await dbContext.Sessions.FirstOrDefaultAsync(item => item.Id == sessionId.Value && item.AccountId == account.Id, cancellationToken);
		}

		maybeItem ??= await dbContext.Sessions
			.Where(item =>
				item.AccountId == account.Id &&
				item.IpAddress == executionContextProvider.IpAddress &&
				item.DeviceInfo == executionContextProvider.UserAgent)
			.FirstOrDefaultAsync(cancellationToken);

		if (maybeItem is null)
		{
			maybeItem = new Session(
				account.Id,
				tokenProvider.CreateRefreshToken(),
				dateTimeProvider.UtcNow.AddDays(options.Value.RefreshTokenExpiredTimeInDays),
				executionContextProvider.UserAgent,
				executionContextProvider.IpAddress);

			await dbContext.Sessions.AddAsync(maybeItem, cancellationToken);
		}
		else
		{
			dbContext.Sessions.Update(
				maybeItem.Update(
					tokenProvider.CreateRefreshToken(),
					dateTimeProvider.UtcNow.AddDays(options.Value.RefreshTokenExpiredTimeInDays),
					executionContextProvider.IpAddress,
					executionContextProvider.UserAgent));
		}

		return maybeItem;
	}
	protected ValueTask<EntityEntry<IntegrationService>> CreateIntegrationService(string value, IntegrationServiceType serviceType, CancellationToken cancellationToken) =>
		dbContext.IntegrationServices.AddAsync(new IntegrationService(serviceType, value), cancellationToken);
	protected ValueTask<EntityEntry<CompanyUser>> CreateCompanyUserAsync(Guid companyId, Guid userId, bool isOwner = false, CancellationToken cancellationToken = default) =>
		dbContext.CompanyUsers.AddAsync(
			new CompanyUser(
				companyId,
				userId,
				isOwner),
			cancellationToken);
}
