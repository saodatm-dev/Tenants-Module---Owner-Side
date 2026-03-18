using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Permissions;
using Core.Domain.Results;
using Core.Domain.RolePermissions;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.Create;

internal sealed class CreateRoleCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<CreateRoleCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
	{
		var permissions = await dbContext.Permissions
			.AsNoTracking()
			.Where(item => command.PermissionIds.Contains(item.Id))
			.Select(item => new { ModuleName = item.Group, PermissionId = item.Id })
			.ToListAsync(cancellationToken);

		if (!permissions.Any())
			return Result.Failure<Guid>(sharedViewLocalizer.PermissionNotFound(nameof(Permission)));

		// roles
		var role = new Role(
				command.Name,
				command.Type,
				executionContextProvider.TenantId);

		// role permissions 
		var rolePermissions = new Dictionary<string, IEnumerable<RolePermission>>();

		foreach (var item in permissions.GroupBy(item => item.ModuleName))
		{
			rolePermissions.Add(
				item.Key,
				item.ToList()
					.Select(p =>
						new RolePermission(
							role.Id,
							p.PermissionId)));
		}

		role.RolePermissionUpsert(rolePermissions);

		await dbContext.Roles.AddAsync(role, cancellationToken);

		await dbContext.RolePermissions.AddRangeAsync(rolePermissions.SelectMany(item => item.Value), cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return role.Id;
	}
}
