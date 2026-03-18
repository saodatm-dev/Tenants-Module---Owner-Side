using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Identity;

internal sealed class InvitationPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionInvitationList = new PermissionType("invitations:list");
	internal static PermissionType PermissionInvitationGetById = new PermissionType("invitations:get-by-id");
	internal static PermissionType PermissionInvitationCreate = new PermissionType("invitations:create");
	internal static PermissionType PermissionInvitationUpdate = new PermissionType("invitations:update");
	internal static PermissionType PermissionInvitationRemove = new PermissionType("invitations:remove");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionInvitationList.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionInvitationGetById.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionInvitationCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionInvitationUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionInvitationRemove.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionInvitationList.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionInvitationGetById.PermissionName,Core.Domain.Roles.RoleType.Client)];
}
