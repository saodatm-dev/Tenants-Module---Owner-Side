using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Banks.Update;

internal sealed class UpdateBankCommandValidator : AbstractValidator<UpdateBankCommand>
{
	public UpdateBankCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankCommand.Id)).Description);

		RuleFor(item => item.Mfo)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankCommand.Mfo)).Description)
			.MaximumLength(20)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateBankCommand.Mfo), 20).Description);

		RuleFor(item => item.Address)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankCommand.Address)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateBankCommand.Address), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(x => x.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(x.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankCommand.Translates)).Description);
	}
}
