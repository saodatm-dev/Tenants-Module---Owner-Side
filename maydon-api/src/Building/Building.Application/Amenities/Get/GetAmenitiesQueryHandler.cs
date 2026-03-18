using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.Get;

internal sealed class GetAmenitiesQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetAmenitiesQuery, PagedList<GetAmenitiesResponse>>
{
	public async Task<Result<PagedList<GetAmenitiesResponse>>> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.AmenityTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Include(item => item.Amenity)
			.Select(item => new GetAmenitiesResponse(
				item.Amenity.Id,
				item.Value,
				item.Amenity.IconUrl))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetAmenitiesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
