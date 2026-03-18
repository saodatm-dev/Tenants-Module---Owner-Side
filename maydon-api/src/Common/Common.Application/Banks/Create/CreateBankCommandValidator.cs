using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Banks.Create;

internal sealed class CreateBankCommandValidator : AbstractValidator<CreateBankCommand>
{
	public CreateBankCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Mfo)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBankCommand.Mfo)).Description)
			.MaximumLength(20)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateBankCommand.Mfo), 20).Description);

		RuleFor(item => item.Address)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBankCommand.Address)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateBankCommand.Address), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBankCommand.Translates)).Description);
	}
}
