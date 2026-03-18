using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Accounts.Events;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Events;

internal sealed class DefaultAccountPreDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<DefaultAccountPreDomainEvent>
{
	public async ValueTask Handle(DefaultAccountPreDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var maybeItems = await dbContext.Accounts
			.Where(item => item.UserId == domainEvent.Account.UserId)
			.ToListAsync(cancellationToken);

		var current = maybeItems.Find(item => item.Id == domainEvent.Account.Id);
		if (current is not null)
			dbContext.Accounts.Update(current.Default());

		var others = maybeItems.Except([current]);
		if (others.Any())
			dbContext.Accounts.UpdateRange(others.Where(item => item is not null).Select(item => item!.NonDefault()));

		await ValueTask.CompletedTask;
	}
}
