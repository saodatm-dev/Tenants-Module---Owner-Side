using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Accounts;
using Identity.Domain.CompanyUsers;
using Identity.Domain.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Events;

internal sealed class AcceptInvitationDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<AcceptInvitationDomainEvent>
{
	public async ValueTask Handle(AcceptInvitationDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		await CreateCompanyUserAsync(domainEvent, cancellationToken);
		await CreateAccountAsync(domainEvent, cancellationToken);
	}

	private async ValueTask CreateCompanyUserAsync(AcceptInvitationDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var exists = await dbContext.CompanyUsers
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.AnyAsync(item => item.CompanyId == domainEvent.CompanyId && item.UserId == domainEvent.UserId, cancellationToken);

		if (!exists)
		{
			await dbContext.CompanyUsers.AddAsync(
				new CompanyUser(
					domainEvent.CompanyId,
					domainEvent.UserId,
					false),
				cancellationToken);
		}
	}

	private async ValueTask CreateAccountAsync(AcceptInvitationDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		// TODO : take account type, it means owner can send invitation in status is host or is guest
		if (!await dbContext.Accounts
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.AnyAsync(item =>
				item.TenantId == domainEvent.CompanyId &&
				item.UserId == domainEvent.UserId &&
				item.RoleId == domainEvent.RoleId,
			cancellationToken))
		{
			await dbContext.Accounts.AddAsync(
				new Account(
					domainEvent.CompanyId,
					domainEvent.UserId)
				.ChangeRole(domainEvent.RoleId),
				cancellationToken);
		}
	}
}
