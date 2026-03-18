using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Leases.GetById;

internal sealed class GetLeaseByIdQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetLeaseByIdQuery, GetLeaseByIdResponse>
{
	public async Task<Result<GetLeaseByIdResponse>> Handle(GetLeaseByIdQuery request, CancellationToken cancellationToken)
	{
		var lease = await dbContext.Leases
			.AsNoTrackingWithIdentityResolution()
			.Where(l => l.Id == request.Id)
			.Include(l => l.Items)
				.ThenInclude(i => i.RealEstate)
			.IgnoreQueryFilters()
			.Select(l => new GetLeaseByIdResponse(
				l.Id,
				l.OwnerId,
				l.AgentId,
				l.ClientId,
				l.StartDate,
				l.EndDate,
				l.PaymentDay,
				l.ContractNumber,
				l.Status.ToString(),
				l.Items.Select(i => new LeaseItemResponse(
					i.Id,
					i.ListingId,
					i.RealEstateId,
					i.RealEstateUnitId,
					dbContext.GetCategoryNameById(i.RealEstate.RealEstateTypeId),
					i.RealEstate.BuildingNumber,
					i.RealEstate.FloorNumber,
					i.RealEstate.RoomsCount,
					i.RealEstate.TotalArea,
					i.RealEstate.LivingArea,
					i.RealEstate.CeilingHeight,
					i.RealEstate.Address,
					i.MonthlyRent,
					i.DepositAmount,
					i.IsMetersIncluded)).ToList()))
			.FirstOrDefaultAsync(cancellationToken);

		if (lease is null)
			return Result<GetLeaseByIdResponse>.None;

		return lease;
	}
}

