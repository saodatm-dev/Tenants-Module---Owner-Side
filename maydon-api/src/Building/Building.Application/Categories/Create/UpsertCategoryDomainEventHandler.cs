using Building.Application.Core.Abstractions.Data;
using Building.Domain.Categories;
using Building.Domain.Categories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.Create;

internal sealed class UpsertCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertCategoryDomainEvent>
{
	public async ValueTask Handle(UpsertCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.CategoryTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.CategoryId == @event.CategoryId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.CategoryTranslates.Update(
						existTranslate.Update(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value));
				else
					await dbContext.CategoryTranslates.AddAsync(
						new CategoryTranslate(
							@event.CategoryId,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value),
						cancellationToken);
			}
		}
		else
		{
			await dbContext.CategoryTranslates.AddRangeAsync(
				@event.Translates
				.Select(item =>
					new CategoryTranslate(
						@event.CategoryId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value)),
				cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
