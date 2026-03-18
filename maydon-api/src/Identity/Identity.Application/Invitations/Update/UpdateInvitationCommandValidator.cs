using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Invitations.Update;

internal sealed class UpdateInvitationCommandValidator : AbstractValidator<UpdateInvitationCommand>
{
	public UpdateInvitationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(UpdateInvitationCommand.Id)).Description);

		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);
	}
}
