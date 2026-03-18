using Common.Application.Core.Abstractions.Data;
using Common.Domain.Currencies.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.Remove;

internal sealed class RemoveCurrencyDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<RemoveCurrencyDomainEvent>
{
	public async ValueTask Handle(RemoveCurrencyDomainEvent @event, CancellationToken cancellationToken)
	{
		var currencyTranslates = await dbContext.CurrencyTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.CurrencyId == @event.Id)
			.ToListAsync(cancellationToken);

		if (currencyTranslates?.Count > 0)
			dbContext.CurrencyTranslates.RemoveRange(currencyTranslates);

		await ValueTask.CompletedTask;
	}
}
