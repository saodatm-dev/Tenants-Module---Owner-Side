using Building.Application.Core.Abstractions.Data;
using Building.Domain.ProductionTypes;
using Building.Domain.ProductionTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ProductionTypes.Create;

internal sealed class CreateOrUpdateProductionTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<CreateOrUpdateProductionTypeDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateProductionTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.ProductionTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.ProductionTypeId == @event.ProductionTypeId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.ProductionTypeTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.ProductionTypeTranslates.AddAsync(new ProductionTypeTranslate(@event.ProductionTypeId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.ProductionTypeTranslates.AddRangeAsync(@event
					.Translates
					.Select(item => new ProductionTypeTranslate(
						@event.ProductionTypeId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value
					)), cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
