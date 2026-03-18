using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Languages.Get;

internal sealed class GetLanguagesQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetLanguagesQuery, PagedList<GetLanguagesResponse>>
{
	public async Task<Result<PagedList<GetLanguagesResponse>>> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Languages
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Name.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.OrderBy(item => item.Order)
			.Select(item => new GetLanguagesResponse(
				item.Id,
				item.Name,
				item.ShortCode,
				item.Order))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetLanguagesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
