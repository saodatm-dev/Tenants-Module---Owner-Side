using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class ComplexPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionComplexCreate = new PermissionType("complexes:create");
	internal static PermissionType PermissionComplexUpdate = new PermissionType("complexes:update");
	internal static PermissionType PermissionComplexRemove = new PermissionType("complexes:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionComplexCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionComplexUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionComplexRemove.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionComplexCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionComplexUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionComplexRemove.PermissionName,Core.Domain.Roles.RoleType.Owner)];
}
