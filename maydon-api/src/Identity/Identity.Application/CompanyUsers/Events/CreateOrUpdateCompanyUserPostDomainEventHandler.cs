namespace Identity.Application.CompanyUsers.Events;

internal sealed class CreateOrUpdateCompanyUserPostDomainEventHandler(IAmazonSNSService amazonSNSService) : IDomainEventHandler<CreateOrUpdateCompanyUserPostDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateCompanyUserPostDomainEvent @event, CancellationToken cancellationToken)
	{
		await amazonSNSService.PublishAsync(
			new UpsertCompanyUserAmazonSNSRequest(
				@event.CompanyUser.Id,
				@event.CompanyUser.CompanyId,
				@event.CompanyUser.UserId,
				@event.CompanyUser.RoleId,
				@event.CompanyUser.IsOwner,
				@event.CompanyUser.IsDirector,
				@event.CompanyUser.IsActive), cancellationToken);

		await ValueTask.CompletedTask;
	}
}
