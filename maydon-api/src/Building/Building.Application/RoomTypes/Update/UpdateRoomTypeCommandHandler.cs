using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RoomTypes.Update;

internal sealed class UpdateRoomTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateRoomTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRoomTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RoomTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoomTypeNotFound(nameof(UpdateRoomTypeCommand.Id)));

		// check language values to exist languages
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateRoomTypeCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.RoomTypes.Update(maybeItem.Update(command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
