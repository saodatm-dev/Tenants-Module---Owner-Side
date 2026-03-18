using Common.Application.Core.Abstractions.Data;
using Common.Application.Districts.Create;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.Update;

internal sealed class UpdateDistrictCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<UpdateDistrictCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateDistrictCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Districts.FindAsync(new object?[] { command.Id }, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.DistrictNotFound(nameof(UpdateDistrictCommand.Id)));

		// check region id 
		if (!await dbContext.Regions.AsNoTracking().AnyAsync(item => item.Id == command.RegionId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.DistrictInvalidRegion(nameof(CreateDistrictCommand.RegionId)));

		// check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateDistrictCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.Districts.Update(maybeItem.Update(command.RegionId, command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
