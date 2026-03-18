using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Rooms.Get;

internal sealed class GetRoomsQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRoomsQuery, PagedList<GetRoomsResponse>>
{
	public async Task<Result<PagedList<GetRoomsResponse>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Rooms
			.Where(item => item.RealEstateId == request.RealEstateId)
			.Include(item => item.RoomType)
			.ThenInclude(item => item.Translates)
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.RoomType.Translates.First().Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.Select(item => new GetRoomsResponse(
				item.Id,
				item.RealEstateId,
				item.RoomType.Translates.First().Value,
				item.Area))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetRoomsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
