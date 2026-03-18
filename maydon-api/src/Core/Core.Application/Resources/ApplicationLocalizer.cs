using Core.Domain.Results;

namespace Core.Application.Resources;

public static partial class ApplicationLocalizer
{
	private const string DateOnlyFormat = "dd.MM.yyyy";

	extension(ISharedViewLocalizer sharedViewLocalizer)
	{
		public Error IsRequired(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IsRequired"]);

		public Error IsEmpty(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IsEmpty"]);

		public Error NoAccess(string parameter) => Error.Validation(parameter, sharedViewLocalizer["NoAccess"]);

		#region Errors
		public Error PasswordIsEmpty(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IsEmpty"]);

		public Error AlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["AlreadyExists"]);

		public Error NotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["NotFound"]);

		public Error InvalidValue(string parameter) => Error.Validation(parameter, sharedViewLocalizer["InvalidValue"]);

		public Error AlreadyHost(string parameter) => Error.Validation(parameter, sharedViewLocalizer["AlreadyHost"]);

		public Error YouAreNotUsingHost(string parameter) => Error.Validation(parameter, sharedViewLocalizer["YouAreNotUsingHost"]);

		public Error MinimalValueHasToBe(string parameter, int length) => Error.Validation(parameter, string.Format(sharedViewLocalizer["MinimalValueHasToBe"], length));

		public Error MaximumValueHasToBe(string parameter, int length) => Error.Validation(parameter, string.Format(sharedViewLocalizer["MaximumValueHasToBe"], length));

		public Error PhoneNumberOrPasswordIncorrect(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_PhoneNumberOrPasswordIncorrect"]);

		public Error UserNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["User_NotFound"]);

		public Error UserIsVerified(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["User_IsVerified"]);

		public Error Unauthorized(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_Unauthorized"]);

		public Error IndividualUserCanNotInvite(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IndividualUserCanNotInvite"]);

		public Error PhoneNumberBlockedUntilTime(string parameter, string time) => Error.Validation(parameter, string.Format(sharedViewLocalizer["PhoneNumberBlockedUntilTime"], time));

		public Error TryAfterTime(string parameter, string time) => Error.Validation(parameter, string.Format(sharedViewLocalizer["TryAfterTime"], time));

		public Error InvalidOtpCode(string parameter) => Error.Validation(parameter, sharedViewLocalizer["InvalidOtpCode"]);

		public Error OnlyIndividualsCanBind(string parameter) => Error.Validation(parameter, sharedViewLocalizer["OnlyIndividualsCanBind"]);

		public Error OwnerCanNotMakeRequestToOwnListing(string parameter) => Error.Validation(parameter, sharedViewLocalizer["OwnerCanNotMakeRequestToOwnListing"]);

		public Error InactiveListingCannotAcceptRequests(string parameter) => Error.Validation(parameter, sharedViewLocalizer["InactiveListingCannotAcceptRequests"]);

		public Error ListingRequestWasAcceptedByOwner(string parameter) => Error.Validation(parameter, sharedViewLocalizer["ListingRequestWasAcceptedByOwner"]);

		public Error ListingRequestAlreadyCancelled(string parameter) => Error.Validation(parameter, sharedViewLocalizer["ListingRequestAlreadyCancelled"]);

		public Error ListingRequestAlreadyRejected(string parameter) => Error.Validation(parameter, sharedViewLocalizer["ListingRequestAlreadyRejected"]);

		public Error WasAcceptedByModerator(string parameter) => Error.Validation(parameter, sharedViewLocalizer["WasAcceptedByModerator"]);

		public Error WasCancelledByUser(string parameter) => Error.Validation(parameter, sharedViewLocalizer["WasCancelledByUser"]);

		public Error WasRejectedByModerator(string parameter) => Error.Validation(parameter, sharedViewLocalizer["WasRejectedByModerator"]);

		public Error WasBlockedByModerator(string parameter) => Error.Validation(parameter, sharedViewLocalizer["WasBlockedByModerator"]);

		#endregion

		#region Otp

		public string RegistrationCodeContent(string code) => string.Format(sharedViewLocalizer["RegistrationCodeContent"], code);
		public string RestorePasswordContent(string code) => string.Format(sharedViewLocalizer["RestorePasswordContent"], code);
		public string ContractDebtContent(string number, DateOnly date, decimal debt) => string.Format(sharedViewLocalizer["ContractDebtContent"], number, date.ToString(DateOnlyFormat), debt);
		public string ContractNewTermContent(string number) => string.Format(sharedViewLocalizer["ContractNewTermContent"], number);
		public string ContractExpiredContent(string number) => string.Format(sharedViewLocalizer["ContractExpiredContent"], number);
		#endregion

		#region User state

		public Error UserStateTimeIsExpired(string key) => Error.Validation(key, sharedViewLocalizer["UserStateTimeIsExpired"]);

		#endregion

		#region Translates	

		public string InvitationContentByUserIdContent(string fullName) => string.Format(sharedViewLocalizer["InvitationContentByUserIdContent"], fullName);

		public string InvitationContentByPhoneNumber(string phoneNumber) => string.Format(sharedViewLocalizer["InvitationContentByPhoneNumber"], phoneNumber);
		#endregion

		#region Storage Errors

		public Error StorageUploadFailed(string parameter) => Error.Failure(parameter, sharedViewLocalizer["Storage_UploadFailed"]);

		public Error StorageDownloadFailed(string parameter) => Error.Failure(parameter, sharedViewLocalizer["Storage_DownloadFailed"]);

		public Error StorageDeleteFailed(string parameter) => Error.Failure(parameter, sharedViewLocalizer["Storage_DeleteFailed"]);

		public Error StorageConnectionError(string parameter) => Error.Failure(parameter, sharedViewLocalizer["Storage_ConnectionError"]);

		public Error MinioApiUploadFailed(string parameter) => Error.Failure(parameter, sharedViewLocalizer["MinioApi_UploadFailed"]);

		public Error MinioApiDeleteFailed(string parameter) => Error.Failure(parameter, sharedViewLocalizer["MinioApi_DeleteFailed"]);

		public Error MinioApiConnectionError(string parameter) => Error.Failure(parameter, sharedViewLocalizer["MinioApi_ConnectionError"]);

		public Error MinioApiInvalidResponse(string parameter) => Error.Failure(parameter, sharedViewLocalizer["MinioApi_InvalidResponse"]);

		#endregion

		#region Entity-Specific Errors - Identity Module

		// User
		public Error UserAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_AlreadyExists"]);
		public Error UserInvalidPhoneNumber(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_InvalidPhoneNumber"]);
		public Error UserInvalidPassword(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_InvalidPassword"]);
		public Error UserAccountNotActive(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_AccountNotActive"]);

		// Role
		public Error RoleNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Role_NotFound"]);
		public Error RoleAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Role_AlreadyExists"]);
		public Error RoleCannotDelete(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Role_CannotDelete"]);

		// Permission
		public Error PermissionNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Permission_NotFound"]);

		// Company
		public Error CompanyNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Company_NotFound"]);
		public Error CompanyAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Company_AlreadyExists"]);
		public Error CompanyInvalidTin(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Company_InvalidTin"]);

		// Account
		public Error AccountNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Account_NotFound"]);
		public Error AccountAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Account_AlreadyExists"]);
		public Error AccountInvalidKey(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Account_InvalidKey"]);
		public Error CannotDeactivateOwnAccount(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Account_CannotDeactivateOwn"]);

		// Invitation
		public Error InvitationNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Invitation_NotFound"]);
		public Error InvitationAlreadyAccepted(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Invitation_AlreadyAccepted"]);
		public Error InvitationAlreadyCancelled(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Invitation_AlreadyCancelled"]);
		public Error InvitationAlreadyRejected(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Invitation_AlreadyRejected"]);
		public Error InvitationExpired(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Invitation_Expired"]);
		public Error InvitationNoPermission(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Invitation_NoPermission"]);

		// OTP Content
		public Error OtpContentNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["OtpContent_NotFound"]);
		public Error OtpContentAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["OtpContent_AlreadyExists"]);

		// Bank Property
		public Error BankPropertyNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["BankProperty_NotFound"]);
		public Error BankPropertyAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["BankProperty_AlreadyExists"]);
		public Error BankPropertyInvalidMfo(string parameter) => Error.Validation(parameter, sharedViewLocalizer["BankProperty_InvalidMfo"]);
		public Error BankPropertyInvalidAccountNumber(string parameter) => Error.Validation(parameter, sharedViewLocalizer["BankProperty_InvalidAccountNumber"]);

		// RefreshToken
		public Error RefreshTokenInvalid(string parameter) => Error.Validation(parameter, sharedViewLocalizer["RefreshToken_Invalid"]);
		public Error RefreshTokenExpired(string parameter) => Error.Validation(parameter, sharedViewLocalizer["RefreshToken_Expired"]);

		// Logo/Image
		public Error LogoNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Logo_NotFound"]);

		// EImzo/OneId
		public Error EImzoInvalidSignature(string parameter) => Error.Validation(parameter, sharedViewLocalizer["EImzo_InvalidSignature"]);
		public Error EImzoCertificateExpired(string parameter) => Error.Validation(parameter, sharedViewLocalizer["EImzo_CertificateExpired"]);
		public Error EImzoAlreadyBound(string parameter) => Error.Validation(parameter, sharedViewLocalizer["EImzo_AlreadyBound"]);
		public Error EImzoLoginNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["EImzo_LoginNotFound"]);
		public Error EImzoAuthFailed(string parameter) => Error.Failure(parameter, sharedViewLocalizer["EImzo_AuthFailed"]);
		public Error EImzoServiceUnavailable(string parameter) => Error.Failure(parameter, sharedViewLocalizer["EImzo_ServiceUnavailable"]);
		public Error OneIdInvalidCode(string parameter) => Error.Validation(parameter, sharedViewLocalizer["OneId_InvalidCode"]);
		public Error OneIdAlreadyRegistered(string parameter) => Error.Validation(parameter, sharedViewLocalizer["OneId_AlreadyRegistered"]);

		// UserState
		public Error UserStateInvalid(string parameter) => Error.Validation(parameter, sharedViewLocalizer["UserState_Invalid"]);

		// OTP
		public Error OtpExpiredOrInvalid(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Otp_ExpiredOrInvalid"]);

		#endregion

		#region Entity-Specific Errors - Building Module

		// Listing
		public Error ListingNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Listing_NotFound"]);
		public Error ListingInactive(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Listing_Inactive"]);
		public Error ListingInvalidCategories(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Listing_InvalidCategories"]);
		public Error ListingInvalidRealEstates(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Listing_InvalidRealEstates"]);

		// ListingRequest
		public Error ListingRequestNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["ListingRequest_NotFound"]);
		public Error ListingRequestNoPermission(string parameter) => Error.Validation(parameter, sharedViewLocalizer["ListingRequest_NoPermission"]);

		// Complex
		public Error ComplexNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Complex_NotFound"]);
		public Error ComplexAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Complex_AlreadyExists"]);

		// Building
		public Error BuildingNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Building_NotFound"]);
		public Error BuildingAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Building_AlreadyExists"]);

		// Floor
		public Error FloorNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Floor_NotFound"]);
		public Error FloorAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Floor_AlreadyExists"]);
		public Error FloorCannotBelongToBoth(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Floor_CannotBelongToBoth"]);
		public Error FloorMustHaveParent(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Floor_MustHaveParent"]);

		// RealEstate
		public Error RealEstateNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["RealEstate_NotFound"]);
		public Error RealEstateNoAccess(string parameter) => Error.Validation(parameter, sharedViewLocalizer["RealEstate_NoAccess"]);
		public Error RealEstateUnitNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["RealEstateUnit_NotFound"]);

		// Category
		public Error CategoryNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Category_NotFound"]);
		public Error CategoryAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Category_AlreadyExists"]);

		// Amenity
		public Error AmenityNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Amenity_NotFound"]);
		public Error AmenityCategoryNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["AmenityCategory_NotFound"]);

		// Renovation
		public Error RenovationNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Renovation_NotFound"]);

		// LandCategory
		public Error LandCategoryNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["LandCategory_NotFound"]);

		// ProductionType
		public Error ProductionTypeNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["ProductionType_NotFound"]);

		// Wishlist
		public Error WishlistNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Wishlist_NotFound"]);
		public Error WishlistAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Wishlist_AlreadyExists"]);

		// MeterType
		public Error MeterTypeNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["MeterType_NotFound"]);
		public Error MeterTypeAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["MeterType_AlreadyExists"]);

		// MeterTariff
		public Error MeterTariffNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["MeterTariff_NotFound"]);
		public Error MeterTariffInvalidMeterType(string parameter) => Error.Validation(parameter, sharedViewLocalizer["MeterTariff_InvalidMeterType"]);

		// RealEstateType
		public Error RealEstateTypeNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["RealEstateType_NotFound"]);
		public Error RealEstateTypeAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["RealEstateType_AlreadyExists"]);
		public Error RealEstateTypeInvalidIcon(string parameter) => Error.Validation(parameter, sharedViewLocalizer["RealEstateType_InvalidIcon"]);

		// RoomType
		public Error RoomTypeNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["RoomType_NotFound"]);
		public Error RoomTypeAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["RoomType_AlreadyExists"]);

		// Room
		public Error RoomNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Room_NotFound"]);

		// Floor
		public Error FloorInvalidPlan(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Floor_InvalidPlan"]);

		#endregion

		#region Entity-Specific Errors - Common Module

		// Region
		public Error RegionNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Region_NotFound"]);
		public Error RegionAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Region_AlreadyExists"]);

		// District
		public Error DistrictNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["District_NotFound"]);
		public Error DistrictInvalidRegion(string parameter) => Error.Validation(parameter, sharedViewLocalizer["District_InvalidRegion"]);

		// Language
		public Error LanguageNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Language_NotFound"]);
		public Error LanguageAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Language_AlreadyExists"]);

		// Currency
		public Error CurrencyNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Currency_NotFound"]);

		// Bank
		public Error BankNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["Bank_NotFound"]);
		public Error BankAlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["Bank_AlreadyExists"]);

		#endregion
	}
}
