using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Accounts;
using Identity.Domain.Accounts.Events;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Events;

public sealed class CreateHostAccountPreDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<CreateHostAccountPreDomainEvent>
{
	public async ValueTask Handle(CreateHostAccountPreDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Accounts
			.FirstOrDefaultAsync(item =>
				item.TenantId == domainEvent.TenantId &&
				item.UserId == domainEvent.UserId &&
				item.IsHost == true
				, cancellationToken);

		if (maybeItem is null)
			await dbContext.Accounts.AddAsync(new Account(domainEvent.TenantId, domainEvent.UserId).ChangeRole(domainEvent.RoleId).CreateHost(), cancellationToken);

		await ValueTask.CompletedTask;
	}
}
