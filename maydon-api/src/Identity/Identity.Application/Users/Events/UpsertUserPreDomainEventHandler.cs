using Core.Domain.Events;
using Identity.Domain.Users.Events;

namespace Identity.Application.Users.Events;

internal sealed class UpsertUserPreDomainEventHandler : IDomainEventHandler<UpsertUserPreDomainEvent>
{
	public async ValueTask Handle(UpsertUserPreDomainEvent @event, CancellationToken cancellationToken)
	{
		await ValueTask.CompletedTask;
	}
}
