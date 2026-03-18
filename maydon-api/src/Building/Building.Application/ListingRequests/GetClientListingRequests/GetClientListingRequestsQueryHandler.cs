using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingRequests;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Get;

internal sealed class GetClientListingRequestsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetClientListingRequestsQuery, PagedList<GetClientListingRequestsResponse>>
{
	public async Task<Result<PagedList<GetClientListingRequestsResponse>>> Handle(GetClientListingRequestsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.ListingRequests
			.AsNoTracking()
			.Clients(executionContextProvider.TenantId)
			.Include(item => item.Listing)
			.OrderBy(item => item.Status)
			.Select(item => new GetClientListingRequestsResponse(
				item.Id,
				item.OwnerId,
				dbContext.GetTenantNameById(item.OwnerId),
				item.Listing.BuildingNumber,
				item.Content,
				item.Status));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetClientListingRequestsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}

