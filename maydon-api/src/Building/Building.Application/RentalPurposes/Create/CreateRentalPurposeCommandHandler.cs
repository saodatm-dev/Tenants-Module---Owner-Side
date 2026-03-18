using Building.Application.Core.Abstractions.Data;
using Building.Domain.RentalPurposes;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RentalPurposes.Create;

internal sealed class CreateRentalPurposeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateRentalPurposeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateRentalPurposeCommand command, CancellationToken cancellationToken)
	{
		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);

			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateRentalPurposeCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		var item = new RentalPurpose(command.Translates);

		await dbContext.RentalPurposes.AddAsync(item, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return item.Id;
	}
}
