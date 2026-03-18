using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.BankProperties.Get;

internal sealed class GetBankPropertiesQueryHandler(
	IIdentityDbContext dbContext) : IQueryHandler<GetBankPropertiesQuery, PagedList<GetBankPropertiesResponse>>
{
	public async Task<Result<PagedList<GetBankPropertiesResponse>>> Handle(GetBankPropertiesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.BankProperties
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.BankName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") ||
				  EF.Functions.Like(item.AccountNumber, $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.OrderBy(item => !item.IsMain)
			.ThenBy(item => item.BankName)
			.Select(item => new GetBankPropertiesResponse(
				item.Id,
				item.BankName,
				item.BankMFO,
				item.AccountNumber,
				item.IsMain,
				item.IsPublic))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetBankPropertiesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
