using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class MeterPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionMeterCreate = new PermissionType("meters:create");
	internal static PermissionType PermissionMeterUpdate = new PermissionType("meters:update");
	internal static PermissionType PermissionMeterRemove = new PermissionType("meters:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionMeterCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionMeterUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionMeterRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
