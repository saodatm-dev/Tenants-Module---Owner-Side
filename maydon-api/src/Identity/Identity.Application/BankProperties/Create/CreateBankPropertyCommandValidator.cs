using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.BankProperties.Create;

internal sealed class CreateBankPropertyCommandValidator : AbstractValidator<CreateBankPropertyCommand>
{
	private const short MfoLength = 5;
	private const short AccountNumberLength = 20;
	public CreateBankPropertyCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.BankId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBankPropertyCommand.BankId)).Description);

		RuleFor(x => x.BankName)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateBankPropertyCommand.BankName)).Description)
			.MaximumLength(200)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateBankPropertyCommand.BankName), 200).Description);

		RuleFor(x => x.BankMfo)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateBankPropertyCommand.BankMfo)).Description);

		RuleFor(item => item.BankMfo)
			.Length(MfoLength, MfoLength)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBankPropertyCommand.BankMfo)).Description);

		RuleFor(x => x.AccountNumber)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateBankPropertyCommand.AccountNumber)).Description);

		RuleFor(x => x.AccountNumber)
			.Length(AccountNumberLength, AccountNumberLength)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateBankPropertyCommand.AccountNumber)).Description);
	}
}
