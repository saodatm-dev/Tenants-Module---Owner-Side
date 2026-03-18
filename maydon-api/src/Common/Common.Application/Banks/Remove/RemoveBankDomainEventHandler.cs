using Common.Application.Core.Abstractions.Data;
using Common.Domain.Banks.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.Remove;

internal sealed class RemoveBankDomainEventHandler(ICommonDbContext dbContext) : IDomainEventHandler<RemoveBankDomainEvent>
{
	public async ValueTask Handle(RemoveBankDomainEvent @event, CancellationToken cancellationToken)
	{
		var bankTranslates = await dbContext.BankTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.BankId == @event.Id)
			.ToListAsync(cancellationToken);

		if (bankTranslates?.Count > 0)
			dbContext.BankTranslates.RemoveRange(bankTranslates);
	}
}
