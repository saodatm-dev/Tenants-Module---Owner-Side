using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Services.AmazonSNS;
using Identity.Domain.Companies.Events;

namespace Identity.Application.Companies.Events;

internal sealed class CreateOrUpdateCompanyPostDomainEventHandler(IAmazonSNSService amazonSNSService) : IDomainEventHandler<CreateOrUpdateCompanyPostDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateCompanyPostDomainEvent @event, CancellationToken cancellationToken)
	{
		await amazonSNSService.PublishAsync(
			new UpsertCompanyAmazonSNSRequest(
				@event.Company.Id,
				@event.Company.Name,
				@event.Company.Tin,
				@event.Company.IsVerified,
				@event.Company.IsActive), cancellationToken);

		await ValueTask.CompletedTask;
	}
}
