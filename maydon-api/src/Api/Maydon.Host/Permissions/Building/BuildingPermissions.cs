using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class BuildingPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionBuildingCreate = new PermissionType("buildings:create");
	internal static PermissionType PermissionBuildingUpdate = new PermissionType("buildings:update");
	internal static PermissionType PermissionBuildingRemove = new PermissionType("buildings:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionBuildingCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionBuildingUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionBuildingRemove.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionBuildingCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionBuildingUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionBuildingRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
