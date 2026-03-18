using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Accounts.Activate;

internal sealed class ActivateAccountCommandValidator : AbstractValidator<ActivateAccountCommand>
{
	public ActivateAccountCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.UserId)
			.NotEmpty().WithMessage(sharedViewLocalizer.IsRequired(nameof(ActivateAccountCommand.UserId)).Description);
	}
}


