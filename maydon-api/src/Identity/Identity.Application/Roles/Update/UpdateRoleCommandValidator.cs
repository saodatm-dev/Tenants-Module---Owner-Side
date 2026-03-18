using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Roles.Update;

internal sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
	public UpdateRoleCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoleCommand.Id)).Description);

		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoleCommand.Name)).Description)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateRoleCommand.Name), 100).Description);

		RuleFor(item => item.PermissionIds)
			.Must(item => item is not null && item.Any())
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRoleCommand.PermissionIds)).Description);
	}
}
