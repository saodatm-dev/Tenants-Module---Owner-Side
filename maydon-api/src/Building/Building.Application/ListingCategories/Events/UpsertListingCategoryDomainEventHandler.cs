using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingCategories;
using Building.Domain.ListingCategories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingCategories.Events;


internal sealed class UpsertListingCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertListingCategoryDomainEvent>
{
	public async ValueTask Handle(UpsertListingCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.ListingCategoryTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.ListingCategoryId == @event.ListingCategoryId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.ListingCategoryTranslates.Update(
						existTranslate.Update(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value));
				else
					await dbContext.ListingCategoryTranslates.AddAsync(
						new ListingCategoryTranslate(
							@event.ListingCategoryId,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value),
						cancellationToken);
			}
		}
		else
		{
			await dbContext.ListingCategoryTranslates.AddRangeAsync(
				@event.Translates
				.Select(item =>
					new ListingCategoryTranslate(
						@event.ListingCategoryId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value)),
				cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
