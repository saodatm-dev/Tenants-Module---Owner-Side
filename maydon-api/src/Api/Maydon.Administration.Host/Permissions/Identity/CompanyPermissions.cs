using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Identity;

internal sealed class CompanyPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionCompanyList = new PermissionType("companies:list");
	internal static PermissionType PermissionCompanyGetById = new PermissionType("companies:get-by-id");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionCompanyList.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionCompanyGetById.PermissionName,Core.Domain.Roles.RoleType.System)];
}
