using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Floors.GetById;

internal sealed class GetFloorByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetFloorByIdQuery, GetFloorByIdResponse>
{
	public async Task<Result<GetFloorByIdResponse>> Handle(GetFloorByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Floors
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Include(item => item.RealEstates)
			.Select(item => new GetFloorByIdResponse(
				item.Id,
				item.Building != null ? item.Building.Number : null,
				dbContext.GetBulkCategoryBuildingTypes(item.RealEstates.Select(item => item.RealEstateTypeId)),
				item.Number,
				item.Type,
				item.Label,
				item.TotalArea,
				item.CeilingHeight,
				item.Plan))
			.AsSplitQuery()
			.FirstOrDefaultAsync(cancellationToken);
	}
}
