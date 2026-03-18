using Building.Application.Categories.Create;
using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingCategories;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingCategories.Create;

internal sealed class CreateListingCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateListingCategoryCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateListingCategoryCommand command, CancellationToken cancellationToken)
	{
		//check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);
		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateCategoryCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		// check parentId 
		if (command.ParentId is not null &&
			command.ParentId != Guid.Empty &&
			!await dbContext.ListingCategories.AsNoTracking().AnyAsync(item => item.Id == command.ParentId.Value, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(command.ParentId)));

		var item = new ListingCategory(
			command.BuildingType,
			command.IconUrl,
			command.Translates,
			command.ShowInMain,
			command.ParentId);

		await dbContext.ListingCategories.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
