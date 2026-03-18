using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class ListingRequestPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionListingRequestCreate = new PermissionType("listing-requests:create");
	internal static PermissionType PermissionListingRequestUpdate = new PermissionType("listing-requests:update");
	internal static PermissionType PermissionListingRequestRemove = new PermissionType("listing-requests:remove");
	internal static PermissionType PermissionListingRequestSend = new PermissionType("listing-requests:send");
	internal static PermissionType PermissionListingRequestReceive = new PermissionType("listing-requests:receive");
	internal static PermissionType PermissionListingRequestAccept = new PermissionType("listing-requests:accept");
	internal static PermissionType PermissionListingRequestReject = new PermissionType("listing-requests:reject");
	internal static PermissionType PermissionListingRequestCancel = new PermissionType("listing-requests:cancel");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		// Client permissions (Individual role)
		new RolePermissionValue(PermissionListingRequestCreate.PermissionName, Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionListingRequestUpdate.PermissionName, Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionListingRequestRemove.PermissionName, Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionListingRequestSend.PermissionName, Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionListingRequestCancel.PermissionName, Core.Domain.Roles.RoleType.Client),
		// Owner permissions
		new RolePermissionValue(PermissionListingRequestReceive.PermissionName, Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionListingRequestAccept.PermissionName, Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionListingRequestReject.PermissionName, Core.Domain.Roles.RoleType.Owner)];
}
