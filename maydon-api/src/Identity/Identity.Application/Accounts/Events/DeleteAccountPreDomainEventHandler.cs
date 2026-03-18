using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Accounts.Events;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Events;

internal sealed class DeleteAccountPreDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<DeleteAccountPreDomainEvent>
{
	public async ValueTask Handle(DeleteAccountPreDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var maybeItems = await dbContext.Accounts
			.Where(item =>
				item.TenantId == domainEvent.TenantId &&
				item.UserId == domainEvent.UserId)
			.ToListAsync(cancellationToken);

		if (maybeItems.Any())
			dbContext.Accounts.RemoveRange(maybeItems);

		await ValueTask.CompletedTask;
	}
}
