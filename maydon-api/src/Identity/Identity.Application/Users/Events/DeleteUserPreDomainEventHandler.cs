using Core.Domain.Events;
using Identity.Domain.Users.Events;

namespace Identity.Application.Users.Events;

internal sealed class DeleteUserPreDomainEventHandler : IDomainEventHandler<DeleteUserPreDomainEvent>
{
	public async ValueTask Handle(DeleteUserPreDomainEvent @event, CancellationToken cancellationToken) =>
		await ValueTask.CompletedTask;
}
