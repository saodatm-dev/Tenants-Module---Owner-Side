using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Roles.Clone;

internal sealed class CloneRoleCommandValidator : AbstractValidator<CloneRoleCommand>
{
	public CloneRoleCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RoleId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CloneRoleCommand.RoleId)).Description);

		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CloneRoleCommand.Name)).Description);
	}
}
