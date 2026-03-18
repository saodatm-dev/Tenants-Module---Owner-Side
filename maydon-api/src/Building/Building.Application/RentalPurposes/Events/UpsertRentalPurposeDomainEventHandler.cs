using Building.Application.Core.Abstractions.Data;
using Building.Domain.RentalPurposes;
using Building.Domain.RentalPurposes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RentalPurposes.Events;

internal sealed class UpsertRentalPurposeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertRentalPurposeDomainEvent>
{
	public async ValueTask Handle(UpsertRentalPurposeDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.RentalPurposeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RentalPurposeId == @event.RentalPurposeId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.RentalPurposeTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.RentalPurposeTranslates.AddAsync(new RentalPurposeTranslate(@event.RentalPurposeId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.RentalPurposeTranslates.AddRangeAsync(@event
					.Translates
					.Select(item => new RentalPurposeTranslate(
						@event.RentalPurposeId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value
					)), cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
