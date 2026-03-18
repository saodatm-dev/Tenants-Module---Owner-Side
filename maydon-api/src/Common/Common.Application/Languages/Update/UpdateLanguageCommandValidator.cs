using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Languages.Update;

internal sealed class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
{
	public UpdateLanguageCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLanguageCommand.Id)).Description);

		RuleFor(item => item.Name)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLanguageCommand.Name)).Description)
			.MaximumLength(100)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateLanguageCommand.Name), 100).Description);

		RuleFor(item => item.ShortCode)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLanguageCommand.ShortCode)).Description)
			.MaximumLength(10)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateLanguageCommand.ShortCode), 10).Description);
	}
}
