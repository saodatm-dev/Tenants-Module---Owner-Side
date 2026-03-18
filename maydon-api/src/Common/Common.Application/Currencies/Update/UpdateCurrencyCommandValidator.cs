using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Currencies.Update;

internal sealed class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
	public UpdateCurrencyCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCurrencyCommand.Id)).Description);

		RuleFor(item => item.Code)
			.NotEmpty()
			.MaximumLength(10)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCurrencyCommand.Code)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCurrencyCommand.Translates)).Description);
	}
}
