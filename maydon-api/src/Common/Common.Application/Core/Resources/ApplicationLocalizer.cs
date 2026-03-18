using Core.Application.Resources;
using Core.Domain.Results;

namespace Common.Application.Core.Resources;

internal static class ApplicationLocalizer
{
	extension(ISharedViewLocalizer sharedViewLocalizer)
	{
		#region Errors
		public Error PasswordIsEmpty(string parameter) => Error.Validation(parameter, sharedViewLocalizer["IsEmpty"]);

		public Error AlreadyExists(string parameter) => Error.Validation(parameter, sharedViewLocalizer["AlreadyExists"]);

		public Error NotFound(string parameter) => Error.NotFound(parameter, sharedViewLocalizer["NotFound"]);

		public Error InvalidValue(string parameter) => Error.Validation(parameter, sharedViewLocalizer["InvalidValue"]);

		public Error NoAccess(string parameter) => Error.Validation(parameter, sharedViewLocalizer["NoAccess"]);

		public Error MinimalValueHasToBe(string parameter, int length) => Error.Validation(parameter, sharedViewLocalizer["MinimalValueHasToBe"]);

		public Error MaximumValueHasToBe(string parameter, int length) => Error.Validation(parameter, sharedViewLocalizer["MaximumValueHasToBe"]);
		#endregion
	}
}
