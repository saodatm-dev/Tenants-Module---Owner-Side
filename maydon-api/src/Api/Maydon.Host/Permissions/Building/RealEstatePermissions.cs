using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class RealEstatePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRealEstateCreate = new PermissionType("realestates:create");
	internal static PermissionType PermissionRealEstateUpdate = new PermissionType("realestates:update");
	internal static PermissionType PermissionRealEstateRemove = new PermissionType("realestates:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRealEstateCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRealEstateUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRealEstateRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
