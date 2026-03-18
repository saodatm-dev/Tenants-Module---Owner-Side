using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;

namespace Building.Application.AmenityCategories.Update;

internal sealed class UpdateAmenityCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateAmenityCategoryCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateAmenityCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.AmenityCategories.FindAsync([command.Id], cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateAmenityCategoryCommand.Id)));

		// check language values to exist languages
		//var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		//for (int i = 0; i < command.Translates.Count; i++)
		//{
		//	var languageValue = command.Translates[i];
		//	var language = languages.Find(item => item.Id == languageValue.LanguageId);
		//	if (language is null)
		//		return Result.Failure<Guid>(sharedViewLocalizer.InvalidValue($"{nameof(CreateRoomTypeCommand.Translates)}.{languageValue.LanguageId}"));

		//	command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		//}


		await dbContext.AmenityCategories.AddAsync(maybeItem.Update(command.Translates), cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
