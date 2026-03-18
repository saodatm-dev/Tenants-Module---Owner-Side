using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class LandCategoryPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionLandCategoryCreate = new PermissionType("landcategories:create");
	internal static PermissionType PermissionLandCategoryUpdate = new PermissionType("landcategories:update");
	internal static PermissionType PermissionLandCategoryRemove = new PermissionType("landcategories:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionLandCategoryCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionLandCategoryUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionLandCategoryRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
