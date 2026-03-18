using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.BankProperties.Update;

internal sealed class UpdateBankPropertyCommandValidator : AbstractValidator<UpdateBankPropertyCommand>
{
	private const short MfoLength = 5;
	private const short AccountNumberLength = 20;
	public UpdateBankPropertyCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankPropertyCommand.Id)).Description);

		RuleFor(item => item.BankId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankPropertyCommand.BankId)).Description);

		RuleFor(item => item.BankName)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateBankPropertyCommand.BankName)).Description)
			.MaximumLength(200)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateBankPropertyCommand.BankName), 200).Description);

		RuleFor(item => item.BankMfo)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateBankPropertyCommand.BankMfo)).Description);

		RuleFor(item => item.BankMfo)
			.Length(MfoLength, MfoLength)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankPropertyCommand.BankMfo)).Description);

		RuleFor(item => item.AccountNumber)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateBankPropertyCommand.AccountNumber)).Description);

		RuleFor(item => item.AccountNumber)
			.Length(AccountNumberLength, AccountNumberLength)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateBankPropertyCommand.AccountNumber)).Description);
	}
}
