using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class FloorPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionFloorCreate = new PermissionType("floors:create");
	internal static PermissionType PermissionFloorUpdate = new PermissionType("floors:update");
	internal static PermissionType PermissionFloorRemove = new PermissionType("floors:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionFloorCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionFloorUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionFloorRemove.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionFloorCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionFloorUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionFloorRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
