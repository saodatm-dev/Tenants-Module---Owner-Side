using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class CategoryPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionCategoryCreate = new PermissionType("categories:create");
	internal static PermissionType PermissionCategoryUpdate = new PermissionType("categories:update");
	internal static PermissionType PermissionCategoryRemove = new PermissionType("categories:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionCategoryCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionCategoryUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionCategoryRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
