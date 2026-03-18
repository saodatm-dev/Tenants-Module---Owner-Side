using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.Update;

internal sealed class UpdateRenovationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateRenovationCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRenovationCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.Renovations.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RenovationNotFound(nameof(UpdateRenovationCommand.Id)));

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateRenovationCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.Renovations.Update(maybeItem.Update(command.Translates));
		await dbContext.SaveChangesAsync(cancellationToken);
		return maybeItem.Id;
	}
}
