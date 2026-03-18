using Building.Application.Core.Abstractions.Data;
using Building.Domain.Renovations;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.Create;

internal sealed class CreateRenovationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateRenovationCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateRenovationCommand command, CancellationToken cancellationToken)
	{
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);

			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateRenovationCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		var item = new Renovation(command.Translates);

		await dbContext.Renovations.AddAsync(item, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return item.Id;
	}
}
