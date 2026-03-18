using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class RealEstateImagePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRealEstateImageUpload = new PermissionType("realestateimages:upload");
	internal static PermissionType PermissionRealEstateImageRemove = new PermissionType("realestateimages:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRealEstateImageUpload.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRealEstateImageRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
