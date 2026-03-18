using Common.Application.Core.Abstractions.Data;
using Common.Domain.Banks;
using Common.Domain.Banks.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.Create;

internal sealed class CreateOrUpdateBankDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<CreateOrUpdateBankDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateBankDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.BankTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.BankId == @event.BankId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.BankTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.BankTranslates.AddAsync(new BankTranslate(@event.BankId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.BankTranslates.AddRangeAsync(@event
				.Translates
				.Select(item =>
					new BankTranslate(
						@event.BankId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value))
				, cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
