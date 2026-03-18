using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class AmenityPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionAmenityList = new PermissionType("amenities:list");
	internal static PermissionType PermissionAmenityById = new PermissionType("amenities:byid");
	internal static PermissionType PermissionAmenityCreate = new PermissionType("amenities:create");
	internal static PermissionType PermissionAmenityUpdate = new PermissionType("amenities:update");
	internal static PermissionType PermissionAmenityRemove = new PermissionType("amenities:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionAmenityList.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionAmenityById.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionAmenityCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionAmenityUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionAmenityRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
