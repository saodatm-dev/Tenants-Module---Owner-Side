using Building.Application.Core.Abstractions.Data;
using Building.Domain.AmenityCategories;
using Building.Domain.AmenityCategories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.AmenityCategories.Create;

internal sealed class UpsertAmenityCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertAmenityCategoryDomainEvent>
{
	public async ValueTask Handle(UpsertAmenityCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.AmenityCategoryTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.AmenityCategoryId == @event.AmenityCategoryId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.AmenityCategoryTranslates.Update(
						existTranslate.Update(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value));
				else
					await dbContext.AmenityCategoryTranslates.AddAsync(
						new AmenityCategoryTranslate(
							@event.AmenityCategoryId,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value),
						cancellationToken);
			}
		}
		else
		{
			await dbContext.AmenityCategoryTranslates.AddRangeAsync(
				@event.Translates
				.Select(item =>
					new AmenityCategoryTranslate(
						@event.AmenityCategoryId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value)),
				cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
