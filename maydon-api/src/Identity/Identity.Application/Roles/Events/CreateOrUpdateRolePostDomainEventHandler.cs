namespace Identity.Application.Roles.Events;

internal sealed class CreateOrUpdateRolePostDomainEventHandler(IAmazonSNSService amazonSNSService) : IDomainEventHandler<CreateOrUpdateRolePostDomainEvent>
{
	private const string IdentityModuleName = "Identity";
	public async ValueTask Handle(CreateOrUpdateRolePostDomainEvent @event, CancellationToken cancellationToken)
	{
		await amazonSNSService.PublishAsync(
			new UpsertRoleAmazonSNSRequest(
				@event.Role.Id,
				@event.Role.TenantId,
				@event.Role.Type,
				@event.Role.IsActive)
			, cancellationToken);

		if (@event.Role.UpsertRolePermissions.Any())
		{
			var upsertRolePermissionRequest =
				@event.Role.UpsertRolePermissions
					.Where(item => item.Key != IdentityModuleName)
					.Select(item =>
						amazonSNSService.PublishAsync(
							new UpsertRolePermissionAmazonSNSRequest(
								item.Key,
								item.Value.Select(rp => new UpsertRolePermissions(
									rp.Id,
									rp.RoleId,
									rp.PermissionId))), cancellationToken).AsTask());


			await Task.WhenAll(upsertRolePermissionRequest);
		}

		if (@event.Role.RemoveRolePermissions.Any())
		{
			var deteleRolePermissionRequest =
				@event.Role.RemoveRolePermissions
					.Where(item => item.Key != IdentityModuleName)
					.Select(item =>
						amazonSNSService.PublishAsync(
							new DeleteRolePermissionAmazonSNSRequest(
								item.Key,
								item.Value.Select(rp => rp.Id)), cancellationToken).AsTask());


			await Task.WhenAll(deteleRolePermissionRequest);
		}

		await ValueTask.CompletedTask;
	}
}
