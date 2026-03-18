using System.Text.RegularExpressions;
using Core.Application.Resources;
using FluentValidation;
using FluentValidation.Results;

namespace Identity.Application.Core.Validators;

internal static class IdentityValidator
{
	private static readonly Regex PhoneNumberRegex = new(@"^[+]?998([2378150]{2}|(9[013-57-9]))\d{7}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly Regex TinRegex = new(@"^\d{9,15}$", RegexOptions.Compiled);
	private static readonly Regex PinflRegex = new(@"^\d{14}$", RegexOptions.Compiled);
	private const int PasswordMinimumLength = 8;
	private const int PasswordMaximumLength = 20;
	private const int NameMaximumLength = 100;
	private const int AddressMaximumLength = 500;

	extension<T>(IRuleBuilder<T, string> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, string> PhoneNumber(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom((value, context) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					var error = sharedViewLocalizer.IsEmpty(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (!PhoneNumberRegex.IsMatch(value))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

			});
		}

		public IRuleBuilderOptionsConditions<T, string> Password(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom((item, context) =>
			{
				if (item is null)
				{
					var error = sharedViewLocalizer.IsEmpty(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (item?.Length < PasswordMinimumLength)
				{
					var error = sharedViewLocalizer.MinimalValueHasToBe(context.PropertyPath, PasswordMinimumLength);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (item?.Length > PasswordMaximumLength)
				{
					var error = sharedViewLocalizer.MaximumValueHasToBe(context.PropertyPath, PasswordMaximumLength);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, string> Tin(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom((value, context) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					var error = sharedViewLocalizer.IsEmpty(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (value?.Length > 15)
				{
					var error = sharedViewLocalizer.MaximumValueHasToBe(context.PropertyPath, 15);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (!string.IsNullOrEmpty(value) && !TinRegex.IsMatch(value))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, string> Pinfl(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom((value, context) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					var error = sharedViewLocalizer.IsEmpty(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (!string.IsNullOrEmpty(value) && !PinflRegex.IsMatch(value))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, string> Name(ISharedViewLocalizer sharedViewLocalizer, bool isRequired = true)
		{
			return ruleBuilder.Custom((value, context) =>
			{
				if (isRequired && string.IsNullOrWhiteSpace(value))
				{
					var error = sharedViewLocalizer.IsEmpty(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}

				if (value?.Length > NameMaximumLength)
				{
					var error = sharedViewLocalizer.MaximumValueHasToBe(context.PropertyPath, NameMaximumLength);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, string> Address(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom((value, context) =>
			{
				if (value?.Length > AddressMaximumLength)
				{
					var error = sharedViewLocalizer.MaximumValueHasToBe(context.PropertyPath, AddressMaximumLength);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}
}

