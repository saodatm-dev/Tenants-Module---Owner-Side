using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Currencies.Create;

internal sealed class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
	public CreateCurrencyCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Code)
			.NotEmpty()
			.MaximumLength(10)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateCurrencyCommand.Code)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateCurrencyCommand.Translates)).Description);
	}
}
