using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Core.Domain.RolePermissions;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.Update;

internal sealed class UpdateRoleCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<UpdateRoleCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Roles
			.Where(item => item.Id == command.Id && item.TenantId == executionContextProvider.TenantId)
			.FirstOrDefaultAsync(cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoleNotFound(nameof(UpdateRoleCommand.Id)));

		var permissions = await dbContext.Permissions
			.AsNoTracking()
			.Select(item => new { ModuleName = item.Instance, PermissionId = item.Id })
			.ToListAsync(cancellationToken);

		if (!permissions.Any())
			return Result.Failure<Guid>(sharedViewLocalizer.PermissionNotFound(nameof(UpdateRoleCommand.PermissionIds)));

		var existRolePermissions = await dbContext.RolePermissions
			.AsNoTracking()
			.Where(item => item.RoleId == command.Id)
			.ToListAsync(cancellationToken);

		// update
		if (existRolePermissions.Any())
		{
			var upsertRolePermissions = new Dictionary<string, IEnumerable<RolePermission>>();

			var createPermissions = command.PermissionIds
				.Where(permissionId => !existRolePermissions.Select(item => item.PermissionId).Contains(permissionId))
				.ToList();

			var modulePermissions = permissions.Where(item => createPermissions.Contains(item.PermissionId));

			foreach (var item in modulePermissions.GroupBy(item => item.ModuleName))
			{
				upsertRolePermissions.Add(
					item.Key,
					item.ToList()
					.Select(p =>
					new RolePermission(
						maybeItem.Id,
						p.PermissionId)));
			}

			if (upsertRolePermissions.Any())
			{
				maybeItem.RolePermissionUpsert(upsertRolePermissions);

				await dbContext.RolePermissions.AddRangeAsync(upsertRolePermissions.SelectMany(item => item.Value), cancellationToken);
			}


			var deleteRolePermissions = new Dictionary<string, IEnumerable<RolePermission>>();

			var deleteModuleRolePermissions = existRolePermissions
				.Where(item => !command.PermissionIds.Contains(item.PermissionId))
				.ToList();

			foreach (var item in deleteModuleRolePermissions.GroupBy(item => item.Permission.Instance))
			{
				deleteRolePermissions.Add(
					item.Key,
					item.ToList());
			}

			if (deleteModuleRolePermissions.Any())
			{
				maybeItem.RolePermissionRemove(deleteRolePermissions);

				dbContext.RolePermissions.RemoveRange(deleteRolePermissions.SelectMany(item => item.Value));
			}
		}
		//create
		else
		{
			// role permissions 
			var rolePermissions = new Dictionary<string, IEnumerable<RolePermission>>();

			foreach (var item in permissions.GroupBy(item => item.ModuleName))
			{
				rolePermissions.Add(
					item.Key,
					item.ToList()
						.Select(p =>
							new RolePermission(
								maybeItem.Id,
								p.PermissionId)));
			}

			if (rolePermissions.Any())
			{
				maybeItem.RolePermissionUpsert(rolePermissions);

				await dbContext.RolePermissions.AddRangeAsync(rolePermissions.SelectMany(item => item.Value), cancellationToken);
			}
		}

		dbContext.Roles.Update(maybeItem.Update(command.Name, maybeItem.Type, maybeItem.TenantId));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
