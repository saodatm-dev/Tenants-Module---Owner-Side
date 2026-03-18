using Common.Application.Core.Abstractions.Data;
using Common.Domain.Currencies;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.Create;

internal sealed class CreateCurrencyCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<CreateCurrencyCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateCurrencyCommand command, CancellationToken cancellationToken)
	{
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateCurrencyCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		var currency = new Currency(command.Code, command.Translates);

		await dbContext.Currencies.AddAsync(currency, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return currency.Id;
	}
}
