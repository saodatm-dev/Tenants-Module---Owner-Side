using Common.Application.Core.Abstractions.Data;
using Common.Domain.Currencies;
using Common.Domain.Currencies.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.Create;

internal sealed class UpsertCurrencyDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<UpsertCurrencyDomainEvent>
{
	public async ValueTask Handle(UpsertCurrencyDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.CurrencyTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.CurrencyId == @event.Id)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.CurrencyTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.CurrencyTranslates.AddAsync(new CurrencyTranslate(@event.Id, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.CurrencyTranslates.AddRangeAsync(@event
				.Translates
				.Select(item =>
					new CurrencyTranslate(
						@event.Id,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value))
				, cancellationToken);
		}
	}
}
