using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RoomTypes.GetById;

internal sealed class GetRoomTypeByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRoomTypeByIdQuery, GetRoomTypeByIdResponse>
{
	public async Task<Result<GetRoomTypeByIdResponse>> Handle(GetRoomTypeByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.RoomTypeTranslates
			.AsNoTracking()
			.Where(item => item.RoomTypeId == request.Id)
			.GroupBy(item => item.RoomTypeId)
			.Select(item => new GetRoomTypeByIdResponse(
				item.Key,
				item.ToList()
					.Select(translate =>
						new LanguageValue(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
