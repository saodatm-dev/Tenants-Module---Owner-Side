using Building.Application.Core.Abstractions.Data;
using Building.Domain.LandCategories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.LandCategories.Remove;

internal sealed class RemoveLandCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveLandCategoryDomainEvent>
{
	public async ValueTask Handle(RemoveLandCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var landCategoryTranslates = await dbContext.LandCategoryTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.LandCategoryId == @event.Id)
			.ToListAsync(cancellationToken);

		if (landCategoryTranslates?.Count > 0)
			dbContext.LandCategoryTranslates.RemoveRange(landCategoryTranslates);

		await ValueTask.CompletedTask;
	}
}
