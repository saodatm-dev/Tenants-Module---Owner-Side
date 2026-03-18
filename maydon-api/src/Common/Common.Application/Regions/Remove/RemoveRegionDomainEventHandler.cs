using Common.Application.Core.Abstractions.Data;
using Common.Domain.Regions.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Application.Regions.Remove;

internal sealed class RemoveRegionDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<RemoveRegionDomainEvent>
{
	public async ValueTask Handle(RemoveRegionDomainEvent @event, CancellationToken cancellationToken)
	{
		var regionTranslates = await dbContext.RegionTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RegionId == @event.Id)
			.ToListAsync(cancellationToken);

		if (regionTranslates?.Count > 0)
			dbContext.RegionTranslates.RemoveRange(regionTranslates);
	}
}
