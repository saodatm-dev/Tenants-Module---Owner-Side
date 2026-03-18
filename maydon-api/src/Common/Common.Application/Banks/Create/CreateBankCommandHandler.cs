using Common.Application.Core.Abstractions.Data;
using Common.Domain.Banks;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.Create;

internal sealed class CreateBankCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<CreateBankCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateBankCommand command, CancellationToken cancellationToken)
	{
		var isExist = await dbContext.Banks
			.AsNoTracking()
			.AnyAsync(item =>
				item.Mfo == command.Mfo &&
				(!string.IsNullOrWhiteSpace(command.Tin) ? item.Tin == command.Tin : true), cancellationToken);

		if (!isExist)
			return Result.Failure<Guid>(sharedViewLocalizer.BankAlreadyExists(nameof(CreateBankCommand.Mfo)));

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateBankCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		var bank = new Bank(
			command.Mfo,
			command.Tin,
			command.PhoneNumber,
			command.Email,
			command.Website,
			command.Address,
			command.Translates);

		await dbContext.Banks.AddAsync(bank, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return bank.Id;
	}
}
