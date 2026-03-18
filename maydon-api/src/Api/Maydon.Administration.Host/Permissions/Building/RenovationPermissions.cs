using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class RenovationPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRenovationCreate = new PermissionType("renovations:create");
	internal static PermissionType PermissionRenovationUpdate = new PermissionType("renovations:update");
	internal static PermissionType PermissionRenovationRemove = new PermissionType("renovations:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRenovationCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRenovationUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRenovationRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
