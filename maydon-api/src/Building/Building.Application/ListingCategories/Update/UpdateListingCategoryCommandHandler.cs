using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingCategories.Update;

internal sealed class UpdateListingCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateListingCategoryCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateListingCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingCategories.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.CategoryNotFound(nameof(UpdateListingCategoryCommand.Id)));

		//check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);
		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateListingCategoryCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		// check parentId 
		if (command.ParentId is not null &&
			command.ParentId != Guid.Empty &&
			!await dbContext.ListingCategories.AsNoTracking().AnyAsync(item => item.Id == command.ParentId.Value, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(command.ParentId)));

		dbContext.ListingCategories.Update(maybeItem.Update(command.BuildingType, command.IconUrl, command.Translates, command.ShowInMain, command.ParentId));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
