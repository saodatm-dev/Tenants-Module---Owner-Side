namespace Identity.Application.CompanyUsers.Events;

internal sealed class DeleteCompanyUserPostDomainEventHandler(IAmazonSNSService amazonSNSService) : IDomainEventHandler<DeleteCompanyUserPostDomainEvent>
{
	public ValueTask Handle(DeleteCompanyUserPostDomainEvent @event, CancellationToken cancellationToken) =>
		amazonSNSService.PublishAsync(new DeleteCompanyAmazonSNSRequest(@event.CompanyUserId), cancellationToken);
}
