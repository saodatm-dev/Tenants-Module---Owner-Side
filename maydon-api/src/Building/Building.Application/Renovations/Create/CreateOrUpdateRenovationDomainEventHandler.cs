using Building.Application.Core.Abstractions.Data;
using Building.Domain.Renovations;
using Building.Domain.Renovations.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.Create;

internal sealed class CreateOrUpdateRenovationDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<CreateOrUpdateRenovationDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateRenovationDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.RenovationTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RenovationId == @event.RenovationId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.RenovationTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.RenovationTranslates.AddAsync(new RenovationTranslate(@event.RenovationId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.RenovationTranslates.AddRangeAsync(@event
					.Translates
					.Select(item => new RenovationTranslate(
						@event.RenovationId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value
					)), cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
