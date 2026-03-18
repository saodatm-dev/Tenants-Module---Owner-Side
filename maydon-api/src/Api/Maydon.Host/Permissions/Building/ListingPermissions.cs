using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class ListingPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionListingCreate = new PermissionType("listings:create");
	internal static PermissionType PermissionListingUpdate = new PermissionType("listings:update");
	internal static PermissionType PermissionListingRemove = new PermissionType("listings:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionListingCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionListingUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionListingRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
