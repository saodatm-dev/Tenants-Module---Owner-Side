using Common.Application.Core.Abstractions.Data;
using Common.Domain.Regions;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.Create;

internal sealed class CreateRegionCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<CreateRegionCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateRegionCommand command, CancellationToken cancellationToken)
	{
		// check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateRegionCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		var region = new Region(command.Translates);

		await dbContext.Regions.AddAsync(region, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return region.Id;
	}
}
