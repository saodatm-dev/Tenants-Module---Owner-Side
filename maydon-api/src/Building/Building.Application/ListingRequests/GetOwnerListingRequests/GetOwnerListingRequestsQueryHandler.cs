using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingRequests;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.GetOwnerListingRequests;

internal sealed class GetOwnerListingRequestsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetOwnerListingRequestsQuery, PagedList<GetOwnerListingRequestsResponse>>
{
	public async Task<Result<PagedList<GetOwnerListingRequestsResponse>>> Handle(GetOwnerListingRequestsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.ListingRequests
			.AsNoTracking()
			.Owners(executionContextProvider.TenantId)
			.Include(item => item.Listing)
			.OrderBy(item => item.Status)
			.Select(item => new GetOwnerListingRequestsResponse(
				item.Id,
				item.ClientId,
				dbContext.GetTenantNameById(item.ClientId),
				dbContext.GetUserPhone(item.ClientId),
				dbContext.GetUserCompanyName(item.ClientId),
				dbContext.GetUserIsVerified(item.ClientId),
				dbContext.GetUserPhoto(item.ClientId),
				item.Listing.BuildingNumber,
				item.Content,
				item.Status));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetOwnerListingRequestsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}

