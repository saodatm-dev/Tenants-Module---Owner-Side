using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class AmenityCategoryPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionAmenityCategoryCreate = new PermissionType("amenitycategories:create");
	internal static PermissionType PermissionAmenityCategoryUpdate = new PermissionType("amenitycategories:update");
	internal static PermissionType PermissionAmenityCategoryRemove = new PermissionType("amenitycategories:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionAmenityCategoryCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionAmenityCategoryUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionAmenityCategoryRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
