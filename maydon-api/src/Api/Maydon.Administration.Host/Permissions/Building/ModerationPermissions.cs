using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Building;

internal sealed class ModerationPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	// listing
	internal static PermissionType PermissionModerationListingGet = new PermissionType("moderation:listing-get");
	internal static PermissionType PermissionModerationListingGetById = new PermissionType("moderation:listing-get-by-id");
	internal static PermissionType PermissionModerationListingAccept = new PermissionType("moderation:listing-accept");
	internal static PermissionType PermissionModerationListingReject = new PermissionType("moderation:listing-reject");
	internal static PermissionType PermissionModerationListingBlock = new PermissionType("moderation:listing-block");

	// real estate
	internal static PermissionType PermissionModerationRealEstateGet = new PermissionType("moderation:realestate-get");
	internal static PermissionType PermissionModerationRealEstateGetById = new PermissionType("moderation:realestate-get-by-id");
	internal static PermissionType PermissionModerationRealEstateAccept = new PermissionType("moderation:realestate-accept");
	internal static PermissionType PermissionModerationRealEstateReject = new PermissionType("moderation:realestate-reject");
	internal static PermissionType PermissionModerationRealEstateBlock = new PermissionType("moderation:realestate-block");

	// real estate unit
	internal static PermissionType PermissionModerationRealEstateUnitGet = new PermissionType("moderation:realestateunit-get");
	internal static PermissionType PermissionModerationRealEstateUnitGetById = new PermissionType("moderation:realestateunit-get-by-id");
	internal static PermissionType PermissionModerationRealEstateUnitAccept = new PermissionType("moderation:realestateunit-accept");
	internal static PermissionType PermissionModerationRealEstateUnitReject = new PermissionType("moderation:realestateunit-reject");
	internal static PermissionType PermissionModerationRealEstateUnitBlock = new PermissionType("moderation:realestateunit-block");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionModerationListingGet.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationListingGetById.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationListingAccept.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationListingReject.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationListingBlock.PermissionName,Core.Domain.Roles.RoleType.System),

		new RolePermissionValue(PermissionModerationRealEstateGet.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateGetById.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateAccept.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateReject.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateBlock.PermissionName,Core.Domain.Roles.RoleType.System),

		new RolePermissionValue(PermissionModerationRealEstateUnitGet.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateUnitGetById.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateUnitAccept.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateUnitReject.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionModerationRealEstateUnitBlock.PermissionName,Core.Domain.Roles.RoleType.System)];
}
