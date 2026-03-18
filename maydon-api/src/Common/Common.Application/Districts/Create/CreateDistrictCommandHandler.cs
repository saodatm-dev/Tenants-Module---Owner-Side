using Common.Application.Core.Abstractions.Data;
using Common.Domain.Districts;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.Create;

internal sealed class CreateDistrictCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<CreateDistrictCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateDistrictCommand command, CancellationToken cancellationToken)
	{
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

		var district = new District(
			command.RegionId,
			command.Translates);

		await dbContext.Districts.AddAsync(district, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return district.Id;
	}
}
