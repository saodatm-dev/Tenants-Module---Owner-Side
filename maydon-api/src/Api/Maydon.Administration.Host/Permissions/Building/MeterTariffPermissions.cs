using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class MeterTariffPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionMeterTariffCreate = new PermissionType("metertariffs:create");
	internal static PermissionType PermissionMeterTariffUpdate = new PermissionType("metertariffs:update");
	internal static PermissionType PermissionMeterTariffRemove = new PermissionType("metertariffs:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionMeterTariffCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionMeterTariffUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionMeterTariffRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
