using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Invitations.Reject;

internal sealed class RejectInvitationCommandValidator : AbstractValidator<RejectInvitationCommand>
{
	public RejectInvitationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RejectInvitationCommand.Id)).Description);

		RuleFor(item => item.Reason)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Reason))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(RejectInvitationCommand.Reason), 500).Description);
	}
}
