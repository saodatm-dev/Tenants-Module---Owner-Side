using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.Get;

internal sealed class GetDistrictsQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetDistrictsQuery, PagedList<GetDistrictsResponse>>
{
	public async Task<Result<PagedList<GetDistrictsResponse>>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.DistrictTranslates
			.Include(item => item.District)
			.ThenInclude(item => item.Region)
			.ThenInclude(item => item.Translates)
			.Where(item =>
				(!string.IsNullOrWhiteSpace(request.Filter)
					? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
					: true) &&
				(request.RegionId != null
					? item.District.RegionId == request.RegionId
					: true))
			.OrderBy(item => item.District.Order)
			.Select(item => new GetDistrictsResponse(
				item.DistrictId,
				item.Value,
				item.District.Region.Translates.First().Value,
				item.District.Order))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetDistrictsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
