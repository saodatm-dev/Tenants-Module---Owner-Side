using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Invitations.Cancel;

internal sealed class CancelInvitationCommandValidator : AbstractValidator<CancelInvitationCommand>
{
	public CancelInvitationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CancelInvitationCommand.Id)).Description);
	}
}
