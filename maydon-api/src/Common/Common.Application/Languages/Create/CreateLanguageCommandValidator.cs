using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Languages.Create;

internal sealed class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
	public CreateLanguageCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLanguageCommand.Name)).Description)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateLanguageCommand.Name), 100).Description);

		RuleFor(item => item.ShortCode)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLanguageCommand.ShortCode)).Description)
			.MaximumLength(10)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateLanguageCommand.ShortCode), 10).Description);
	}
}
