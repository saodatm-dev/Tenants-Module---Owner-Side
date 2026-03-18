using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.Update;

internal sealed class UpdateCurrencyCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<UpdateCurrencyCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateCurrencyCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.Currencies.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.CurrencyNotFound(nameof(UpdateCurrencyCommand.Id)));

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateCurrencyCommand.Translates)}.{languageValue.LanguageId}"));
			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.Currencies.Update(maybeItem.Update(command.Code, command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
