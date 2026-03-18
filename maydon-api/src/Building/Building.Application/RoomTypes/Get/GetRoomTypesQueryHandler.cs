using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RoomTypes.Get;

internal sealed class GetRoomTypesQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRoomTypesQuery, PagedList<GetRoomTypesResponse>>
{
	public async Task<Result<PagedList<GetRoomTypesResponse>>> Handle(GetRoomTypesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.RoomTypeTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Select(item => new GetRoomTypesResponse(
				item.RoomTypeId,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetRoomTypesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
