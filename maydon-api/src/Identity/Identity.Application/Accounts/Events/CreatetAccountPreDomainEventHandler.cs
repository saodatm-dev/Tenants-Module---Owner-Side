using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Events;

public sealed class CreatetAccountPreDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<CreateAccountPreDomainEvent>
{
	public async ValueTask Handle(CreateAccountPreDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Accounts
			.FirstOrDefaultAsync(item =>
				item.TenantId == domainEvent.TenantId &&
				item.UserId == domainEvent.UserId &&
				item.Type != domainEvent.Type, cancellationToken);

		if (maybeItem is null)
			await dbContext.Accounts.AddAsync(
				new Account(
					domainEvent.TenantId,
					domainEvent.UserId,
					domainEvent.Type)
				.ChangeRole(domainEvent.RoleId), cancellationToken);

		await ValueTask.CompletedTask;
	}
}
