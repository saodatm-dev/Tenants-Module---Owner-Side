using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.Get;

internal sealed class GetBanksQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetBanksQuery, PagedList<GetBanksResponse>>
{
	public async Task<Result<PagedList<GetBanksResponse>>> Handle(GetBanksQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.BankTranslates
			.Where(item =>
			(!string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true))
			.Include(item => item.Bank)
			.OrderBy(item => item.Bank.Order)
			.Select(item => new GetBanksResponse(
				item.BankId,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetBanksResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
