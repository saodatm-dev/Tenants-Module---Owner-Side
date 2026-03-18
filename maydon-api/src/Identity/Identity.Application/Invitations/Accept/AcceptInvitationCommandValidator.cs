using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Invitations.Accept;

internal sealed class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
	public AcceptInvitationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(AcceptInvitationCommand.Id)).Description);
	}
}
