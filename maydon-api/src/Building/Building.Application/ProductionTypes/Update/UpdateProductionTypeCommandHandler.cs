using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ProductionTypes.Update;

internal sealed class UpdateProductionTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateProductionTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateProductionTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.ProductionTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ProductionTypeNotFound(nameof(UpdateProductionTypeCommand.Id)));

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateProductionTypeCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.ProductionTypes.Update(maybeItem.Update(command.Translates));
		await dbContext.SaveChangesAsync(cancellationToken);
		return maybeItem.Id;
	}
}
