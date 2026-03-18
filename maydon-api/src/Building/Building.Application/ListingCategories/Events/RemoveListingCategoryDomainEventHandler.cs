using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingCategories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingCategories.Events;

internal sealed class RemoveListingCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveListingCategoryDomainEvent>
{
	public async ValueTask Handle(RemoveListingCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var translates = await dbContext.ListingCategoryTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.ListingCategoryId == @event.Id)
			.ToListAsync(cancellationToken);

		if (translates?.Count > 0)
			dbContext.ListingCategoryTranslates.RemoveRange(translates);

		await ValueTask.CompletedTask;
	}
}
