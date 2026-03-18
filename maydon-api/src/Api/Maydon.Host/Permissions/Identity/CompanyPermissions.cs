using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Identity;

internal sealed class CompanyPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionCompanyList = new PermissionType("companies:list");
	internal static PermissionType PermissionCompanyGetById = new PermissionType("companies:get-by-id");
	internal static PermissionType PermissionCompanyCreate = new PermissionType("companies:create");
	internal static PermissionType PermissionCompanyUpdate = new PermissionType("companies:update");
	internal static PermissionType PermissionCompanyLogo = new PermissionType("companies:logo");
	internal static PermissionType PermissionCompanyRemove = new PermissionType("companies:remove");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionCompanyList.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionCompanyGetById.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionCompanyCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionCompanyUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionCompanyLogo.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionCompanyRemove.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionCompanyList.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionCompanyGetById.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionCompanyCreate.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionCompanyUpdate.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionCompanyLogo.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionCompanyRemove.PermissionName,Core.Domain.Roles.RoleType.Client)];
}
