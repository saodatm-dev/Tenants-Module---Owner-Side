using Common.Application.Core.Abstractions.Data;
using Common.Domain.Regions;
using Common.Domain.Regions.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.Create;

internal sealed class UpsertRegionDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<UpsertRegionDomainEvent>
{
	public async ValueTask Handle(UpsertRegionDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.RegionTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RegionId == @event.RegionId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.RegionTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.RegionTranslates.AddAsync(new RegionTranslate(@event.RegionId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.RegionTranslates.AddRangeAsync(@event
				.Translates
				.Select(item =>
					new RegionTranslate(
						@event.RegionId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value))
				, cancellationToken);
		}
	}
}
