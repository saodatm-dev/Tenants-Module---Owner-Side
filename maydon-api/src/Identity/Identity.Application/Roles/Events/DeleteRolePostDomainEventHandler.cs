namespace Identity.Application.Roles.Events;

internal sealed class DeleteRolePostDomainEventHandler(IAmazonSNSService amazonSNSService) : IDomainEventHandler<DeleteRolePostDomainEvent>
{
	public ValueTask Handle(DeleteRolePostDomainEvent @event, CancellationToken cancellationToken) =>
		amazonSNSService.PublishAsync(new DeleteRoleAmazonSNSRequest(@event.RoleId), cancellationToken);
}
