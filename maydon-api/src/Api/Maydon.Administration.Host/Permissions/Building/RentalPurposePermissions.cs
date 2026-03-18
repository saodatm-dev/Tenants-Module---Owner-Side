using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class RentalPurposePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionRentalPurposeList = new PermissionType("rental-purposes:list");
	internal static PermissionType PermissionRentalPurposeCreate = new PermissionType("rental-purposes:create");
	internal static PermissionType PermissionRentalPurposeUpdate = new PermissionType("rental-purposes:update");
	internal static PermissionType PermissionRentalPurposeRemove = new PermissionType("rental-purposes:remove");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRentalPurposeList.PermissionName, Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRentalPurposeCreate.PermissionName, Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRentalPurposeUpdate.PermissionName, Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRentalPurposeRemove.PermissionName, Core.Domain.Roles.RoleType.System)];
}
