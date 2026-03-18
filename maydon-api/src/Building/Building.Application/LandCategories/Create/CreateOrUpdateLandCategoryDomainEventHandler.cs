using Building.Application.Core.Abstractions.Data;
using Building.Domain.LandCategories;
using Building.Domain.LandCategories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.LandCategories.Create;

internal sealed class CreateOrUpdateLandCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<CreateOrUpdateLandCategoryDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateLandCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.LandCategoryTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.LandCategoryId == @event.LandCategoryId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.LandCategoryTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.LandCategoryTranslates.AddAsync(new LandCategoryTranslate(@event.LandCategoryId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.LandCategoryTranslates.AddRangeAsync(@event
					.Translates
					.Select(item => new LandCategoryTranslate(
						@event.LandCategoryId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value
					)), cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
