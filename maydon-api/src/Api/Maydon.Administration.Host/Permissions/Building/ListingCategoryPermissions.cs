using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class ListingCategoryPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionListingCategoryCreate = new PermissionType("listingcategories:create");
	internal static PermissionType PermissionListingCategoryUpdate = new PermissionType("listingcategories:update");
	internal static PermissionType PermissionListingCategoryRemove = new PermissionType("listingcategories:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionListingCategoryCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionListingCategoryUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionListingCategoryRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
