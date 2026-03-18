using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.LandCategories.Update;

internal sealed class UpdateLandCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateLandCategoryCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateLandCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.LandCategories.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.LandCategoryNotFound(nameof(UpdateLandCategoryCommand.Id)));

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateLandCategoryCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.LandCategories.Update(maybeItem.Update(command.Translates));
		await dbContext.SaveChangesAsync(cancellationToken);
		return maybeItem.Id;
	}
}
