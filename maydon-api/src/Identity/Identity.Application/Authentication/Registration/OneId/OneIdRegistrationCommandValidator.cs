using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Registration.OneId;

internal sealed class OneIdRegistrationCommandValidator : AbstractValidator<OneIdRegistrationCommand>
{
	public OneIdRegistrationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Code)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(OneIdRegistrationCommand.Code)).Description);
	}
}
