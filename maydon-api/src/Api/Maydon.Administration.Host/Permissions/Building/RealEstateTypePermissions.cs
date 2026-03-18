using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class RealEstateTypePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionRealEstateTypeCreate = new PermissionType("realestatetypes:create");
	internal static PermissionType PermissionRealEstateTypeUpdate = new PermissionType("realestatetypes:update");
	internal static PermissionType PermissionRealEstateTypeRemove = new PermissionType("realestatetypes:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRealEstateTypeCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRealEstateTypeUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRealEstateTypeRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
