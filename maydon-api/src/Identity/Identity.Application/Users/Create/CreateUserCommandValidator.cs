using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Users.Create;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
	public CreateUserCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);

		RuleFor(x => x.Password)
			.Password(sharedViewLocalizer);

		RuleFor(x => x.Tin)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(CreateUserCommand.Tin)).Description)
			.MaximumLength(15)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateUserCommand.Tin), 15).Description);

		RuleFor(x => x.FirstName)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(CreateUserCommand.FirstName)).Description)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateUserCommand.FirstName), 100).Description);

		RuleFor(x => x.LastName)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(CreateUserCommand.LastName)).Description)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateUserCommand.LastName), 100).Description);

		RuleFor(x => x.MiddleName)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateUserCommand.MiddleName), 100).Description)
			.When(x => !string.IsNullOrEmpty(x.MiddleName));
	}
}
