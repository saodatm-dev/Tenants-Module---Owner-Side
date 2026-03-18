using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Rooms.GetById;

internal sealed class GetRoomByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRoomByIdQuery, GetRoomByIdResponse>
{
	public async Task<Result<GetRoomByIdResponse>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Rooms
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Include(item => item.RoomType)
			.ThenInclude(item => item.Translates)
			.Select(item => new GetRoomByIdResponse(
				item.Id,
				item.RealEstateId,
				item.RoomType.Translates.First().Value,
				item.Area))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
