using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class RealEstateUnitPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRealEstateUnitCreate = new PermissionType("realestateunits:create");
	internal static PermissionType PermissionRealEstateUnitUpdate = new PermissionType("realestateunits:update");
	internal static PermissionType PermissionRealEstateUnitRemove = new PermissionType("realestateunits:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRealEstateUnitCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRealEstateUnitUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionRealEstateUnitRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
