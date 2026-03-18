using Common.Application.Core.Abstractions.Data;
using Common.Domain.Districts.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.Remove;

internal sealed class RemoveDistrictDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<RemoveDistrictDomainEvent>
{
	public async ValueTask Handle(RemoveDistrictDomainEvent @event, CancellationToken cancellationToken)
	{
		var translates = await dbContext.DistrictTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.DistrictId == @event.Id)
			.ToListAsync(cancellationToken);

		if (translates?.Count > 0)
			dbContext.DistrictTranslates.RemoveRange(translates);
	}
}
