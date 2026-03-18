using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Accounts.Deactivate;

internal sealed class DeactivateAccountCommandValidator : AbstractValidator<DeactivateAccountCommand>
{
	public DeactivateAccountCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.UserId)
			.NotEmpty().WithMessage(sharedViewLocalizer.IsRequired(nameof(DeactivateAccountCommand.UserId)).Description);
	}
}


