using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Roles.Remove;

internal sealed class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
{
	public RemoveRoleCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRoleCommand.Id)).Description);
	}
}
