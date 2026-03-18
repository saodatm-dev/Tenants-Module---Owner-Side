using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class ProductionTypePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionProductionTypeCreate = new PermissionType("productiontypes:create");
	internal static PermissionType PermissionProductionTypeUpdate = new PermissionType("productiontypes:update");
	internal static PermissionType PermissionProductionTypeRemove = new PermissionType("productiontypes:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionProductionTypeCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionProductionTypeUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionProductionTypeRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
