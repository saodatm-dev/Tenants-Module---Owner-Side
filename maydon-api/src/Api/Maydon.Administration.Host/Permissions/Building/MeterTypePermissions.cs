using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class MeterTypePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionMeterTypeCreate = new PermissionType("metertypes:create");
	internal static PermissionType PermissionMeterTypeUpdate = new PermissionType("metertypes:update");
	internal static PermissionType PermissionMeterTypeRemove = new PermissionType("metertypes:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionMeterTypeCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionMeterTypeUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionMeterTypeRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
