using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Invitations.Remove;

internal sealed class RemoveInvitationCommandValidator : AbstractValidator<RemoveInvitationCommand>
{
	public RemoveInvitationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveInvitationCommand.Id)).Description);
	}
}
