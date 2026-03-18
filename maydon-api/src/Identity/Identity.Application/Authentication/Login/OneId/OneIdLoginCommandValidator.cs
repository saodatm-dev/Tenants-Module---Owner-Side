using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Login.OneId;

internal sealed class OneIdLoginCommandValidator : AbstractValidator<OneIdLoginCommand>
{
	public OneIdLoginCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Code)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(OneIdLoginCommand.Code)).Description);
	}
}
