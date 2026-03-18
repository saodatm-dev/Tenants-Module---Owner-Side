using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Leases.Get;

internal sealed class GetLeasesQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetLeasesQuery, PagedList<GetLeasesResponse>>
{
	public async Task<Result<PagedList<GetLeasesResponse>>> Handle(GetLeasesQuery request, CancellationToken cancellationToken)
	{
		var baseQuery = dbContext.Leases
			.Include(l => l.Items)
			.IgnoreQueryFilters()
			.AsNoTracking();
		
		var leases = await baseQuery
				.Skip(request.Page)
				.Take(request.PageSize)
				.ToListAsync(cancellationToken);
	
		int totalCount = await baseQuery.CountAsync(cancellationToken);

		var responsesPage = leases.Select(lease => new GetLeasesResponse(
			lease.Id,
			lease.OwnerId,
			lease.ClientId,
			lease.StartDate,
			lease.EndDate,
			lease.PaymentDay,
			lease.Status.ToString(),
			lease.Items.Count,
			lease.Items.FirstOrDefault()?.RealEstate?.Address,
			Money.FromSom(lease.Items.Sum(i => i.MonthlyRent.Amount))))
			.ToList();

		return new PagedList<GetLeasesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}

