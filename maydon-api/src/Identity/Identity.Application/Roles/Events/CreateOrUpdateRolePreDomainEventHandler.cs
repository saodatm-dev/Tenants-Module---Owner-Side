using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.Events;

internal sealed class CreateOrUpdateRolePreDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<CreateOrUpdateRolePreDomainEvent>
{
	private const string IdentityModuleName = "Identity";
	private Guid identityModuleId = Guid.Empty;
	public async ValueTask Handle(CreateOrUpdateRolePreDomainEvent @event, CancellationToken cancellationToken)
	{
		var identityModule = await dbContext.Modules.AsNoTracking().FirstOrDefaultAsync(item => item.Name == IdentityModuleName, cancellationToken);
		if (identityModule is not null)
			identityModuleId = identityModule.Id;

		await this.UpdateIdentityRolePermissionsAsync(@event, cancellationToken);

		await ValueTask.CompletedTask;
	}

	private async ValueTask UpdateIdentityRolePermissionsAsync(CreateOrUpdateRolePreDomainEvent @event, CancellationToken cancellationToken)
	{
		var existItems = await dbContext.RolePermissions.Where(item => item.RoleId == @event.RoleId).ToListAsync(cancellationToken);

		var identityPermissions = @event.ModulePermissions
				.Where(modulePermission => modulePermission.ModuleId == identityModuleId)
				.SelectMany(modulePermission => modulePermission.PermissionIds)
				.ToList();

		if (identityPermissions?.Any() == true)
		{
			if (existItems?.Count > 0)
			{
				// update
				var createPermissions = identityPermissions
					.Where(permissionId => !existItems.Select(item => item.PermissionId).Contains(permissionId))
					.ToList();

				var deleteRolePermissions = existItems
					.Where(item => !identityPermissions.Contains(item.PermissionId))
					.ToList();

				if (createPermissions?.Any() == true)
					await dbContext.RolePermissions.AddRangeAsync(createPermissions.Select(permissionId => new Domain.RolePermissions.RolePermission(@event.RoleId, permissionId)), cancellationToken);

				if (deleteRolePermissions?.Any() == true)
					dbContext.RolePermissions.RemoveRange(deleteRolePermissions);
			}
			else
			{
				await dbContext.RolePermissions.AddRangeAsync(identityPermissions
					.Select(permissionId =>
						new Domain.RolePermissions.RolePermission(
							@event.RoleId,
							permissionId))
					, cancellationToken);
			}
		}

		await ValueTask.CompletedTask;
	}
}
