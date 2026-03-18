using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Invitations.Create;

internal sealed class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
	public CreateInvitationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);
	}
}
