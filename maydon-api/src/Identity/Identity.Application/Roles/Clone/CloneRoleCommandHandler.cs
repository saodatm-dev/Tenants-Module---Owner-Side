using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Permissions;
using Core.Domain.Results;
using Core.Domain.RolePermissions;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.Clone;

internal sealed class CloneRoleCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<CloneRoleCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CloneRoleCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Roles
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.Id == command.RoleId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoleNotFound(nameof(CloneRoleCommand.RoleId)));

		var permissions = await dbContext.RolePermissions
			.AsNoTracking()
			.Where(item => item.RoleId == command.RoleId)
			.Include(item => item.Permission)
			.Select(item => new { ModuleName = item.Permission.Instance, PermissionId = item.PermissionId })
			.ToListAsync(cancellationToken);

		if (!permissions.Any())
			return Result.Failure<Guid>(sharedViewLocalizer.PermissionNotFound(nameof(Permission)));

		var role = new Role(
				command.Name,
				maybeItem.Type,
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
