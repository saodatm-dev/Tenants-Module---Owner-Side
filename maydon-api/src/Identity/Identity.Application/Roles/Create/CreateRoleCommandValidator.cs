using Core.Application.Resources;
using Core.Domain.Roles;
using FluentValidation;


namespace Identity.Application.Roles.Create;

internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
	public CreateRoleCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoleCommand.Name)).Description)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateRoleCommand.Name), 100).Description);

		RuleFor(item => item.Type)
			.NotEmpty()
			.Must(item => RoleType.System <= item && item <= RoleType.Owner)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoleCommand.Type)).Description);

		RuleFor(item => item.PermissionIds)
			.Must(item => item is not null && item.Any())
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRoleCommand.PermissionIds)).Description);
	}
}
