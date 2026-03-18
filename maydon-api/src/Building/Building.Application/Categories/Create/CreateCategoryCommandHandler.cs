using Building.Application.Core.Abstractions.Data;
using Building.Domain.Categories;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.Create;

internal sealed class CreateCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateCategoryCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
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

		var item = new Category(
			command.BuildingType,
			command.IconUrl,
			command.Translates);

		await dbContext.Categories.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
