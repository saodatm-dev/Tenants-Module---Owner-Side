using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.Get;

internal sealed class GetCurrenciesQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetCurrenciesQuery, PagedList<GetCurrenciesResponse>>
{
	public async Task<Result<PagedList<GetCurrenciesResponse>>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.CurrencyTranslates
			.Include(item => item.Currency)
			.Where(item => string.IsNullOrWhiteSpace(request.Filter) || EF.Functions.Like(item.Value.ToLower(), $"{request.Filter.ToLowerInvariant()}%"))
			.OrderBy(item => item.Currency.Order)
			.Select(item => new GetCurrenciesResponse(
				item.CurrencyId,
				item.Currency.Code,
				item.Value,
				item.Currency.Order))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetCurrenciesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
