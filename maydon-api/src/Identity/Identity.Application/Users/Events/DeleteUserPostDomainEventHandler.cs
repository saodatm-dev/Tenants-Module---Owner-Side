using Core.Domain.Events;
using Identity.Domain.Users.Events;

namespace Identity.Application.Users.Events;

internal sealed class DeleteUserPostDomainEventHandler : IDomainEventHandler<DeleteUserPostDomainEvent>
{
	public async ValueTask Handle(DeleteUserPostDomainEvent @event, CancellationToken cancellationToken) =>
		await ValueTask.CompletedTask;
}
