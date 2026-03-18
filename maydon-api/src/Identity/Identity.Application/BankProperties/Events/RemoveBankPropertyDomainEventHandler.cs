using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.BankProperties.Events;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.BankProperties.Events;

internal sealed class RemoveMainBankPropertyDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<RemoveMainBankPropertyDomainEvent>
{
	public async ValueTask Handle(RemoveMainBankPropertyDomainEvent @event, CancellationToken cancellationToken)
	{
		var tenantBankProperties = await dbContext.BankProperties
			.Where(item => item.Id != @event.Id && item.TenantId == @event.TenantId)
			.ToListAsync(cancellationToken);

		if (tenantBankProperties.Any())
		{
			var firstBankProperty = tenantBankProperties.FirstOrDefault();
			if (firstBankProperty is not null)
				dbContext.BankProperties.Update(firstBankProperty.EnableMain());
		}

		await ValueTask.CompletedTask;
	}
}
