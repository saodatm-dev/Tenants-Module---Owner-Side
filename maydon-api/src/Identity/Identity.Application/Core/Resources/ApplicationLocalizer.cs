using Core.Application.Resources;
using Core.Domain.Results;

namespace Identity.Application.Core.Resources;

public static class ApplicationLocalizer
{
	extension(ISharedViewLocalizer sharedViewLocalizer)
	{
		#region Errors
		public Error PasswordIsEmpty(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IsEmpty"]);

		public Error AlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["AlreadyExists"]);

		public Error NotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["NotFound"]);

		public Error InvalidValue(string parameter) => Error.Validation(parameter, sharedViewLocalizer["InvalidValue"]);

		public Error AlreadyHost(string parameter) => Error.Validation(parameter, sharedViewLocalizer["AlreadyHost"]);

		public Error YouAreNotUsingHost(string parameter) => Error.Validation(parameter, sharedViewLocalizer["YouAreNotUsingHost"]);

		public Error MinimalValueHasToBe(string parameter, int length) => Error.Validation(parameter, sharedViewLocalizer["MinimalValueHasToBe"]);

		public Error MaximumValueHasToBe(string parameter, int length) => Error.Validation(parameter, sharedViewLocalizer["MaximumValueHasToBe"]);

		public Error PhoneNumberOrPasswordIncorrect(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_PhoneNumberOrPasswordIncorrect"]);

		public Error UserNotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["User_NotFound"]);

		public Error Unauthorized(string parameter) => Error.Validation(parameter, sharedViewLocalizer["User_Unauthorized"]);

		public Error IndividualUserCanNotInvite(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IndividualUserCanNotInvite"]);

		public Error PhoneNumberBlockedUntilTime(string parameter, string time) => Error.Validation(parameter, string.Format(sharedViewLocalizer["PhoneNumberBlockedUntilTime"], time));

		public Error TryAfterTime(string parameter, string time) => Error.Validation(parameter, string.Format(sharedViewLocalizer["TryAfterTime"], time));

		public Error InvalidOtpCode(string parameter) => Error.Validation(parameter, sharedViewLocalizer["InvalidOtpCode"]);

		public Error OnlyIndividualsCanBind(string parameter) => Error.Validation(parameter, sharedViewLocalizer["OnlyIndividualsCanBind"]);
		#endregion


		#region Translates	

		public string InvitationContentByUserIdContent(string fullName) => string.Format(sharedViewLocalizer["InvitationContentByUserIdContent"], fullName);

		public string InvitationContentByPhoneNumber(string phoneNumber) => string.Format(sharedViewLocalizer["InvitationContentByPhoneNumber"], phoneNumber);
		#endregion
	}
}
