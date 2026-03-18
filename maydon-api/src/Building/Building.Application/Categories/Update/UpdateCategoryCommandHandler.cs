using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.Update;

internal sealed class UpdateCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateCategoryCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Categories.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.CategoryNotFound(nameof(UpdateCategoryCommand.Id)));

		// check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);
		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateCategoryCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.Categories.Update(
			maybeItem.Update(
				command.BuildingType,
				command.IconUrl,
				command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
