using Common.Application.Core.Abstractions.Data;
using Common.Domain.Districts;
using Common.Domain.Districts.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.Create;

internal sealed class UpsertDistrictDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<UpsertDistrictDomainEvent>
{
	public async ValueTask Handle(UpsertDistrictDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.DistrictTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.DistrictId == @event.DistrictId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.DistrictTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
				{
					await dbContext.DistrictTranslates.AddAsync(
						new DistrictTranslate(
							@event.DistrictId,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value)
						, cancellationToken);
				}
			}
		}
		else
		{
			await dbContext.DistrictTranslates.AddRangeAsync(@event
				.Translates
				.Select(item =>
					new DistrictTranslate(
						@event.DistrictId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value))
				, cancellationToken);
		}
	}
}
