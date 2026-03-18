using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class RoomPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRoomPermissionCreate = new PermissionType("rooms:create");
	internal static PermissionType PermissionRoomPermissionUpdate = new PermissionType("rooms:update");
	internal static PermissionType PermissionRoomPermissionRemove = new PermissionType("rooms:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRoomPermissionCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRoomPermissionUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRoomPermissionRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
