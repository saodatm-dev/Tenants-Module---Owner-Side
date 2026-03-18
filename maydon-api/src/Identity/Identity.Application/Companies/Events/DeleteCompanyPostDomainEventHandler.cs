namespace Identity.Application.Companies.Events;

internal sealed class DeleteCompanyPostDomainEventHandler(IAmazonSNSService amazonSNSService) : IDomainEventHandler<DeleteCompanyPostDomainEvent>
{
	public ValueTask Handle(DeleteCompanyPostDomainEvent @event, CancellationToken cancellationToken) =>
		amazonSNSService.PublishAsync(new DeleteCompanyAmazonSNSRequest(@event.CompanyId), cancellationToken);
}
