using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.Update;

internal sealed class UpdateRegionCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<UpdateRegionCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRegionCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Regions.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RegionNotFound(nameof(UpdateRegionCommand.Id)));

		// check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateRegionCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.Regions.Update(maybeItem.Update(command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
