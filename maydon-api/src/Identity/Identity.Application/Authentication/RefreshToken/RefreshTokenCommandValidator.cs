using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
	public RefreshTokenCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RefreshToken)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RefreshTokenCommand.RefreshToken)).Description);
	}
}
