using Building.Application.Core.Abstractions.Data;
using Building.Domain.Amenities;
using Building.Domain.Amenities.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.Events;

internal sealed class UpsertAmenityDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertAmenityDomainEvent>
{
	public async ValueTask Handle(UpsertAmenityDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.AmenityTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.AmenityId == @event.AmenityId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.AmenityTranslates.Update(
						existTranslate.Update(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value));
				else
					await dbContext.AmenityTranslates.AddAsync(
						new AmenityTranslate(
							@event.AmenityId,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value),
						cancellationToken);
			}
		}
		else
		{
			await dbContext.AmenityTranslates.AddRangeAsync(
				@event.Translates
				.Select(item =>
					new AmenityTranslate(
						@event.AmenityId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value)),
				cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
