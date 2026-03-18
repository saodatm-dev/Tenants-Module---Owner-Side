using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Authorize.OneId;

internal sealed class OneIdAuthCommandValidator : AbstractValidator<OneIdAuthCommand>
{
	public OneIdAuthCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Code)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(OneIdAuthCommand.Code)).Description);
	}
}
