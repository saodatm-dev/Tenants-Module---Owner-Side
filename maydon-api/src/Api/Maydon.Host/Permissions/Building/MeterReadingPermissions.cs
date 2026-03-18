using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class MeterReadingPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionMeterReadingUpsert = new PermissionType("meterreadings:upsert");
	internal static PermissionType PermissionMeterReadingRemove = new PermissionType("meterreadings:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionMeterReadingUpsert.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionMeterReadingRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
