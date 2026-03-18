using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class RoomTypePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRoomTypeCreate = new PermissionType("roomtypes:create");
	internal static PermissionType PermissionRoomTypeUpdate = new PermissionType("roomtypes:update");
	internal static PermissionType PermissionRoomTypeRemove = new PermissionType("roomtypes:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRoomTypeCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRoomTypeUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRoomTypeRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
