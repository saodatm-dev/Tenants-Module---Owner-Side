using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.Update;

internal sealed class UpdateAmenityCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateAmenityCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateAmenityCommand command, CancellationToken cancellationToken)
	{
		var existingItem = await dbContext.Amenities.FindAsync([command.Id], cancellationToken);
		if (existingItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateAmenityCommand.Id)));

		if (!await dbContext.AmenityCategories.AsNoTracking().AnyAsync(item => item.Id == command.AmenityCategoryId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateAmenityCommand.AmenityCategoryId)));

		//// check language values to exist languages
		//var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		//for (int i = 0; i < command.Translates.Count; i++)
		//{
		//	var languageValue = command.Translates[i];
		//	var language = languages.Find(item => item.Id == languageValue.LanguageId);
		//	if (language is null)
		//		return Result.Failure<Guid>(sharedViewLocalizer.InvalidValue($"{nameof(UpdateAmenityCommand.Translates)}.{languageValue.LanguageId}"));

		//	command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		//}

		dbContext.Amenities.Update(
			existingItem.Update(
				command.AmenityCategoryId,
				command.IconUrl,
				command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return existingItem.Id;
	}
}
