using Building.Application.Core.Abstractions.Data;
using Building.Domain.Listings;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Listings.Get;

internal sealed class GetModerationListingsQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetModerationListingsQuery, PagedList<GetModerationListingsResponse>>
{
	public async Task<Result<PagedList<GetModerationListingsResponse>>> Handle(GetModerationListingsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Listings
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => !request.ModerationStatus.HasValue || item.ModerationStatus == request.ModerationStatus.Value)
			.Select(item => new GetModerationListingsResponse(
				item.Id,
				item.OwnerId,
				item.ComplexName,
				item.BuildingNumber,
				item.Address,
				dbContext.GetListingCategoryNamesByIds(item.CategoryIds),
				item.TotalArea,
				item.RoomsCount,
				item.PriceForMonth,
				item.PricePerSquareMeter,
				item.Description,
				item.Status,
				item.ModerationStatus,
				item.CreatedAt,
				item.RealEstate.Images != null && item.RealEstate.Images.Any() ? item.RealEstate.Images.Select(image => image.ObjectName) : null));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		// Resolve image URLs
		var resolvedItems = new List<GetModerationListingsResponse>(responsesPage.Count);
		foreach (var item in responsesPage)
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.ObjectNames, cancellationToken);
			resolvedItems.Add(item with { ObjectNames = resolvedImages });
		}

		return new PagedList<GetModerationListingsResponse>(resolvedItems, request.Page, request.PageSize, totalCount);
	}
}
