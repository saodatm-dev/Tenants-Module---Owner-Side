using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Authentication.Registration.CheckPhoneNumber;

internal sealed class CheckPhoneNumberCommandValidator : AbstractValidator<CheckPhoneNumberCommand>
{
	public CheckPhoneNumberCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);
	}
}
